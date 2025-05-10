using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;
using Core.Services;
using Rendering.Services;
using Rendering.Views;
using Rendering.Visible;

namespace Rendering.Entities;

public class Structure : Renderable
{
    // Static Data //
    public readonly static List<Structure> Structures = [];
    // Render Data //
    public readonly List<DrawingContext.PushedState> Matrixes = [];
    // Animation Data //
    public Animator AnimController = new()
    {
        Looped = true
    };
    public required string? AnimationName;
    // Constructors //
    [SetsRequiredMembers]
    public Structure(Rect Hitbox, string? AnimationName = null) : base(Hitbox)
    {
        // Initialize //
        this.Hitbox = Hitbox;
        this.AnimationName = AnimationName;
        // Checks //
        if (AnimationName is not null)
            _ = AnimController.Play(AnimationName);
        // Add Reference //
        Structures.Add(this);
        // Connections //
        RunService.Update += OnUpdate;
    }
    [SetsRequiredMembers]
    public Structure(Point Position, Size Size, string? AnimationName = null) : this(new Rect(), AnimationName)
    {
        this.Position = Position;
        this.Size = Size;
    }
    // Event Methods //
    public void OnUpdate(object? sender, UpdateEventArgs eventData) { }
    public override void OnRender(GuiService guiService, DrawingContext context)
    {
        // Data //
        LocalPlayer? localPlayer = WorldService.LocalPlayer;
        // Conditions //
        if (localPlayer is null)
            return;
        if (localPlayer.AttachedCamera is null)
            return;
        if (!localPlayer.AttachedCamera.IsWithinSight(Hitbox))
            return;
        // Camera Transform //
        PushTransform(context, localPlayer.AttachedCamera.OffsetMatrix);
        // Draw //
        if (AnimationName is not null)
        {
            IImage? frame = AnimController.Frame;
            // Condtitions //
            if (frame is null)
                return;
            // Draw //
            context.DrawImage(frame, Hitbox);
        }
        else
            context.DrawRectangle(Brushes.White, null, Hitbox);
        // Reset Matrix //
        PopMatrix();
    }
    // Disposal Methods //
    public override void Delete()
    {
        Structures.Remove(this);
        base.Delete();
    }
}