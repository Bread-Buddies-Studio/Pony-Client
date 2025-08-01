using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using Core.Extensions;
using Core.Services;
using Rendering.Entities;
using Rendering.Services;
using Rendering.Views;

namespace Rendering.Visible;

public class InteractPrompt : OverheadText
{
    // Public Data //
    public double DetectionRange = 250;
    // Base Methods //
    public double LocalPlayerDistance()
    {
        // Conditions //
        ArgumentNullException.ThrowIfNull(WorldService.LocalPlayer);
        // Calculate //
        return Speaker.Position.DistanceFrom(WorldService.LocalPlayer.Position);
    }
    // Override Data //
    public override bool IsAlwaysOnTop => true;
    public override FormattedText FormattedText => new(Message, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Typeface.Default, 40, Brush)
    {
        TextAlignment = TextAlignment.Center,
    };
    // Events //
    public event EventHandler<EventArgs>? Interact;
    // Constructor //
    [SetsRequiredMembers]
    public InteractPrompt(Entity Speaker) : base(Speaker, "!")
    {
        InputService.KeyDown += OnKeyDown;
    }
    // Event Methods //
    public void OnKeyDown(object? sender, KeyEventArgs eventData)
    {
        // Conditions //
        if (!IsVisible)
            return;
        if (eventData.Key != InputService.Controls["interact"])
            return;
        // Success //
        Interact?.Invoke(this, EventArgs.Empty);
    }

    public override void OnUpdate(object? sender, UpdateEventArgs eventData)
    {
        // Update //
        base.OnUpdate(sender, eventData);
        // Conditions //
        if (WorldService.LocalPlayer is null)
            return;
        // Settings //
        this.IsVisible = LocalPlayerDistance() <= DetectionRange;
    }

    public override void OnRender(GuiService guiService, DrawingContext context)
    {
        // Conditions //
        if (WorldService.CurrentCamera is null)
            return;
        // Render //
        base.OnRender(guiService, context);
        PushTransform(context, WorldService.CurrentCamera.OffsetMatrix);
        context.DrawRectangle(null, new Pen(Brush, thickness: 3), Hitbox.WithWidth(Size.Height).WithX(Position.X - Size.Height / 2 + Size.Width / 2));
        PopMatrix();
    }
}