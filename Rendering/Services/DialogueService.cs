using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Core.Extensions;
using Core.Services;
using Rendering.Effects;
using Rendering.Views;

namespace Rendering.Services;

public static class DialogueService
{
    // Data //
    public static readonly Typewriter Writer = new(
                Foreground: new SolidColorBrush(Colors.White),
                BaseTypeface: Typeface.Default,
                Hitbox: new());
    // Constructors //
    static DialogueService()
    {
        RunService.Update += OnUpdate;

        _ = new NervousEffect(Writer, 2);
        _ = new SineEffect(Writer, 5, 3);

        Writer.Text = "[size=30]Name of Character![\\size]\n" +
            " Dialogue!";
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
}
