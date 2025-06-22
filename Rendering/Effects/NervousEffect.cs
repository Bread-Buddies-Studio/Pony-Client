using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Rendering.Views;
using Rendering.Visible;

namespace Rendering.Effects;

[method: SetsRequiredMembers]
internal class NervousEffect(
    string Text,
    IBrush Foreground,
    Typeface BaseTypeface,
    FlowDirection BaseFlowDirection = FlowDirection.LeftToRight, 
    double TextSize = 12) : Renderable
{
    // Instance Data //
    public required string Text = Text;
    public required Typeface BaseTypeface = BaseTypeface;
    public required FlowDirection BaseFlowDirection = BaseFlowDirection;
    public required IBrush Foreground = Foreground;
    public required double TextSize = TextSize;

    // Private Methods //
    private FormattedText GetFormat(string text) =>
        new(
            text,
            CultureInfo.CurrentCulture,
            BaseFlowDirection,
            BaseTypeface,
            TextSize,
            Foreground
        );
    private FormattedText GetFormat(char character) => GetFormat(character.ToString());
    private FormattedText GetFormat(int index) => GetFormat(Text[index]);
    
    private double GetLineHeight(string text) => GetFormat(text).Height;
    private double GetLineHeight(string[] words) => GetLineHeight(string.Join(' ', words));
    private double GetLineHeight(Range range) => GetLineHeight(Text[range]);
    // Event Methods //
    public override void OnRender(GuiService guiService, DrawingContext context)
    {
        string[] words = Text.Split(' ');
        int lineStart = 0;
        double xOffset = 0;
        double yOffset = 0;
        double spaceWidth = GetFormat(' ').Width;

        foreach ((int index, string word) in words.Index())
        {
            FormattedText formattedWord = GetFormat(word);
            // Checks //
            if (xOffset + formattedWord.Width > Size.Width)
            {
                // New Line //
                yOffset += GetLineHeight(new Range(lineStart, index));
                xOffset = 0;
                // Set Line Begin //
                lineStart = index;
            }
            // Draw Word //
            context.DrawText(formattedWord, Position + new Point(xOffset, yOffset));
            // Done //
            xOffset += formattedWord.Width + spaceWidth;
        }
    }
}