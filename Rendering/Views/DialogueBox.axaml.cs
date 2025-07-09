using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.VisualTree;
using Rendering.Effects;
using Rendering.Entities;
using Rendering.Services;

namespace Rendering.Views;

public partial class DialogueBox : UserControl
{
    public required Typewriter Writer;
    public DialogueBox()
    {
        InitializeComponent();

        Writer =
            new(
                Foreground: this.Foreground ?? new SolidColorBrush(255),
                BaseTypeface: Typeface.Default,
                Hitbox: this.Bounds,
                BaseFlowDirection: this.FlowDirection,
                TextSize: this.FontSize)
            {
                BackgroundColor = Colors.White
            };

        Task.Run(async () =>
        {
            await Task.Delay(5000);
            while (WorldService.LocalPlayer is null)
                continue;
            await Write(WorldService.LocalPlayer, "Testing testing 123");
        });
    }

    public async Task Write(Entity speaker, string message)
    {
        Writer.Hitbox = this.Bounds;
        foreach (char letter in message)
        {
            // Extract Data //
            if (!speaker.CharacterDelays.TryGetValue(letter, out int delay))
                delay = speaker.CharacterDelays[' '];
            // Edit Message //
            Writer.Text += letter;
            Debug.WriteLine($"Position: {Writer.Position}");
            Debug.WriteLine($"Size: {Writer.Size}");
            // Delay //
            await Task.Delay(delay);
        }
    }
}