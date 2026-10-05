using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Markup;
using Avalonia;
using Avalonia.Media;
using Core.Extensions;
using Rendering.Services;
using Rendering.Views;
using Rendering.Visible;
using Core.Services;

namespace Rendering.Effects;

[method: SetsRequiredMembers]
public class Typewriter(
    IBrush Foreground,
    Typeface BaseTypeface,
    Rect Hitbox,
    FlowDirection BaseFlowDirection = FlowDirection.LeftToRight,
    double TextSize = 12,
    Thickness Padding = default
    ) : Renderable(Hitbox)
{
    // Matrixes //
    public readonly List<DrawingContext.PushedState> WordMatrixes = [];
    public readonly List<DrawingContext.PushedState> LetterMatrixes = [];
    // Effects //
    public readonly List<Effect> Effects = [];
    // Public Data //
    public required string Text 
    { 
        get;
        set
        {
            field = value;
            BBCodeMatches = Text.ParseBBCode();
            FormatCache = [.. value.Select((char letter, int index) => GetCacheFormat(index))];
        }
    } = string.Empty;
    public required Typeface BaseTypeface = BaseTypeface;
    public required FlowDirection BaseFlowDirection = BaseFlowDirection;
    public required IBrush Foreground = Foreground;
    public required double TextSize = TextSize;
    public required Color BackgroundColor = Colors.DarkGray;
    public required Thickness Padding = Padding;
    // Private Data //
    [AllowNull]
    internal MatchCollection BBCodeMatches;
    [AllowNull]
    private FormattedText[] FormatCache;
    // Override Data //
    public override sealed bool IsAlwaysOnTop => true;
    public override sealed bool AlwaysShows => true;
    // Public Methods //
    public void AddEffect(Effect effect)
    {
        // Conditions //
        ArgumentNullException.ThrowIfNull(effect);
        // Add Effect //
        Effects.Add(effect);
    }
    public bool RemoveEffect(Effect effect)
    {
        // Conditions //
        ArgumentNullException.ThrowIfNull(effect);
        // Remove Effect //
        return Effects.Remove(effect);
    }
    // Private Methods //
    private FormattedText GetCacheFormat(int characterIndex)
    {
        // Conditions //
        if (BBCodeMatches.IsIndexInMatch(characterIndex))
        {
            Match? containerMatch = BBCodeMatches.FindClosestPreviousMatch(characterIndex);
            if (containerMatch is not null && containerMatch.Index + containerMatch.Length < Text.Length)
                characterIndex = containerMatch.Index + containerMatch.Length;
        }
        // Formatting //
        FormattedText formattedText = GetFormat(Text[characterIndex]);
        // Activate Code //
        foreach ((string code, var action) in BBCodeService.BBCodeActions)
        {
            // Conditions //
            if (!BBCodeMatches.IsIndexWithin(characterIndex, code))
                continue;
            Match? closestPreviousMatch = BBCodeMatches.FindClosestPreviousStartingMatch(characterIndex, code);
            string internalString = closestPreviousMatch?.GetInternalString() ?? string.Empty;
            // Success //
            action.Invoke(formattedText, internalString);
        }

        // Success //
        return formattedText;
    }
    private FormattedText GetFormat(string text) => new(
        text,
        CultureInfo.CurrentCulture,
        BaseFlowDirection,
        BaseTypeface,
        TextSize,
        Foreground
    );
    private FormattedText GetFormat(int characterIndex) => FormatCache[characterIndex.Clamp0(FormatCache.Length - 1)];
    private FormattedText GetFormat(char character) => GetFormat(character.ToString());
    
    private double GetLineHeight(Range range)
    {
        double biggestHeight = 0;
        double length = range.End.Value - range.Start.Value;
        // Conditions //
        if (length is 0)
            return 0;
        // Iterate //
        for (int characterIndex = range.Start.Value; characterIndex < range.End.Value; characterIndex++)
        {
            // Conditions //
            if (BBCodeMatches.IsIndexInMatch(characterIndex))
                continue; // Skip if index is in a BBCode match
            // Get Format //
            FormattedText format = GetFormat(characterIndex);
            // Check Height //
            if (format.Height > biggestHeight)
                biggestHeight = format.Height;
        }
        // Return Height //
        return biggestHeight;
    }
    
    private void ApplyWordEffects(DrawingContext context, int index, string word)
    {
        foreach (Effect effect in Effects)
        {
            DrawingContext.PushedState[]? matrixes = effect.WordMatrix(context, index, word);
            // Conditions //
            if (matrixes is null)
                continue; // Skip if no matrixes are returned
            // Add Matrixes //
            WordMatrixes.AddRange(matrixes);

        }
    }
    private void ApplyLetterEffects(DrawingContext context, int index, char letter)
    {
        foreach (Effect effect in Effects)
        {
            DrawingContext.PushedState[]? matrixes = effect.LetterMatrix(context, index, letter);
            // Conditions //
            if (matrixes is null)
                continue; // Skip if no matrixes are returned
            // Add Matrixes //
            LetterMatrixes.AddRange(matrixes);
            
        }
    }

    private double GetSpaceWidth(int characterIndex)
    {
        int characterIndex2 = characterIndex + 2;
        double fontSize;
        // Checks //
        if (BBCodeMatches.IsIndexInMatch(characterIndex2))
        {
            Match? containerMatch = BBCodeMatches.FindClosestPreviousMatch(characterIndex2);
            if (containerMatch is not null && containerMatch.Index + containerMatch.Length < Text.Length)
                characterIndex2 = containerMatch.Index + containerMatch.Length;
        }

        if (BBCodeMatches.IsIndexWithin(characterIndex2, "size"))
        {
            Match? sizeMatch = BBCodeMatches.FindClosestPreviousStartingMatch(characterIndex2, "size");
            if (sizeMatch is not null)
            {
                string? sizeString = sizeMatch.GetInternalString();
                if (sizeString is null)
                    fontSize = TextSize; // Default to TextSize if no size is found
                else
                    fontSize = double.Parse(sizeString);
            }
            else
                fontSize = TextSize;
        }
        else fontSize = TextSize;

        return fontSize / 6;
    }
    private double GetWordWidth(int firstLetterIndex, int wordLength)
    {
        // Init //
        double totalWidth = 0;
        // Iterate //
        for (int index = 0; index < wordLength; index++)
        {
            // Conditions //
            if (BBCodeMatches.IsIndexInMatch(firstLetterIndex + index))
                continue; // Skip if index is in a BBCode match
            // Add Width //
            totalWidth += GetFormat(firstLetterIndex + index).Width;
        }
        // Return Total Width //
        return totalWidth;
    }
    // Draw Methods //
    private void DrawWord(DrawingContext context, int wordIndex, int firstLetterIndex, string word)
    {
        // Apply Word Effects //
        ApplyWordEffects(context, wordIndex, word);
        // Define Letter Spacing //
        double lastCharacterSpacing = 0;
        // Iterate Letters //
        foreach ((int index, char letter) in word.Index())
        {
            // Init //
            int characterIndex = firstLetterIndex + index;
            // Conditions //
            if (BBCodeMatches.IsIndexInMatch(characterIndex))
                continue;
            // Get Formatted Character //
            FormattedText formattedChar = GetFormat(characterIndex);
            // Apply Letter Effects //
            ApplyLetterEffects(context, characterIndex, word[index]);
            // Render //
            context.DrawText(formattedChar, Position + new Point(lastCharacterSpacing, 0));
            // Additional Offset //
            lastCharacterSpacing += formattedChar.Width;
            // Reset Matrix //
            PopMatrix([.. LetterMatrixes]);
            LetterMatrixes.Clear();
        }
        // Reset Word Matrix //
        PopMatrix([.. WordMatrixes]);
        WordMatrixes.Clear();
    }
    public void DrawBackground(DrawingContext context)
    {
        // Conditions //
        if (BackgroundColor == Colors.Transparent)
            return;
        // Tools //
        Brush fill = new SolidColorBrush(BackgroundColor);
        Pen border = new(new SolidColorBrush(BackgroundColor));

        // Draw //
        context.DrawRectangle(
            brush: fill,
            pen: border,
            rect: Hitbox,
            radiusX: 0,
            radiusY: 0
        );
    }
    // Event Methods //
    public override void OnRender(GuiService guiService, DrawingContext context)
    {
        DrawBackground(context);
        // Foreground //
        string[] words = Text.Trim().Split(' ');
        // Position Data //
        int lineStart = 0;
        double xOffset = Padding.Left;
        double yOffset = Padding.Top;
        // Index Data //
        int characterIndex = 0;
        // Iterate through Words //
        foreach ((int wordIndex, string word) in words.Index())
        {

            // Calculations //
            string baseWord = word.RemoveBBCode();
            string nextBaseWord = word.RemoveBBCode();

            double? spaceWidth = wordIndex != words.Length - 1 ?
                GetSpaceWidth(characterIndex + word.Length - 1) : null;
            // Conditions //
            if (word is "" or " ")
            {
                // Conditions //
                if (spaceWidth is null)
                    continue;
                // Draw Space //
                xOffset += (double)spaceWidth;
                characterIndex++;
                // Continue to Next Word //
                continue;
            }

            // Word Formatting //
            double wordWidth = GetWordWidth(characterIndex, word.Length);
            // Checks //
            if (xOffset + wordWidth > Size.Width - Padding.Right || (wordIndex > 0 && words[wordIndex - 1].Contains('\n')))
            {
                // New Line //
                yOffset += GetLineHeight(new Range(lineStart, characterIndex));
                xOffset = Padding.Left;
                // Set Line Begin //
                lineStart = characterIndex;
            }
            // Offset //
            PushTransform(context, new Matrix(1, 0, 0, 1, xOffset, yOffset));
            // Draw Word //
            DrawWord(context, wordIndex, characterIndex, word);
            // Reset Matrixes //
            PopMatrix();
            // Indexing //
            characterIndex += word.Length + (spaceWidth is null ? 0 : 1);
            // Add Space Width to Offset //
            if (spaceWidth is not null) // If not on a new line then add space width to offset
                xOffset += wordWidth + (double)spaceWidth;
        }
    }
}