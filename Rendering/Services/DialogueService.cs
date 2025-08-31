using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using Core.Extensions;
using Core.Services;
using Rendering.Effects;
using Rendering.Views;

namespace Rendering.Services;

public static class DialogueService
{
    // Dictionaries //
    public static readonly Dictionary<char, int> CharacterDelays = new() // (ms) //
    {
        ['\0'] = 10, // Default //
        ['.'] = 750
    };
    // Data //
    public static readonly Typewriter Writer = new(
                Foreground: new SolidColorBrush(Colors.White),
                BaseTypeface: Typeface.Default,
                Hitbox: new(),
                Padding: new(5));

    private static CancellationTokenSource CancelManager = new();
    // Constructors //
    static DialogueService()
    {
        RunService.Update += OnUpdate;
        InputService.KeyDown += OnKeyDown;

        _ = new NervousEffect(Writer);
        _ = new FlickerEffect(Writer);
        _ = new SineEffect(Writer);
        
        Writer.Text = "the [sine=1 5][nervous=1][b][color=red]blood[\\color][\\b][\\nervous][\\sine] runs thick...";/*Name of Character![\\size]\n" +
                                                                                                                     * 
            " Dialogue!";*/
    }
    // Base Methods //
    /// <remarks>
    /// VERY BUGGY
    /// </remarks>
    /// <param name="text"></param>
    public static void SetText(string text)
    {
        // Conditions //
        if (GuiService.Window is null)
            return;
        // Settings //
        Writer.Text = text;
    }
    public static void AppendLetter(char letter) => Writer.Text += letter;
    public static async Task Typewrite(string text)
    {
        // Cancel //
        await CancelManager.CancelAsync();
        CancelManager.Dispose();
        CancelManager = new();
        // Scan Text for BBCode //
        Writer.Text = string.Empty;
        MatchCollection BBCodeMatches = text.ParseBBCode();

        foreach (Match match in BBCodeMatches)
        {
            // Conditions //
            if (CancelManager.IsCancellationRequested)
                return;
            // Settings //
            Writer.Text += match.Value;
        }
        // Data // 
        foreach ((int index, char letter) in text.Index())
        {
            // Conditions //
            if (CancelManager.IsCancellationRequested)
                return; // If the cancellation is requested, exit the method //
            if (BBCodeMatches.IsIndexInMatch(index))
                continue; // If the index is in a BBCode match, skip the delay //
            // Insert Letter //
            if (BBCodeMatches.Last().Index < index)
                AppendLetter(letter); 
            else
                Writer.Text = Writer.Text.Insert(index, letter.ToString());
            // Get Delay Amount //
            if (!CharacterDelays.TryGetValue(letter, out int letterDelay))
                letterDelay = CharacterDelays['\0'];
            // Delay //
            await Task.Delay(letterDelay, CancelManager.Token);
        }
    }
    // Event Methods //
    private static void OnUpdate(object? sender, UpdateEventArgs eventData)
    {
        // Conditions //
        if (GuiService.Window is null)
            return;
        // Data //
        Size windowSize = GuiService.Window.Bounds.Size;
        Writer.Size = windowSize
            .WithWidth(windowSize.Width / 3)
            .WithHeight(windowSize.Height / 4.25);
        Writer.Position = Writer.Position
            .WithX(windowSize.Width / 3)
            .WithY(windowSize.Height * .75);
    }

    private static void OnKeyDown(object? _, KeyEventArgs eventData)
    {
        // Conditions //
        if (eventData.Key != InputService.Controls["toggledialoguebox"])
            return; // If the dialogue is hidden, do not toggle visibility //
        // Checks //
        Writer.IsVisible = !Writer.IsVisible;
    }
}