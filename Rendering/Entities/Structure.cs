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
using Core.Interfaces;
using Avalonia.Media.Imaging;

namespace Rendering.Entities;

public class Structure : Renderable, ICollidable
{
    // Static Data //
    public readonly static List<Structure> Structures = [];
    // Render Data //
    public readonly List<DrawingContext.PushedState> Matrixes = [];
    public Bitmap? Sprite;
    // Collisions //
    public bool IsCollidable { get; set; }
    public bool IsAnchored { get; set; }
    public override byte Layer => 0;
    // Constructors //
    [SetsRequiredMembers]
    public Structure(Bitmap? Sprite, Rect Hitbox) : base(Hitbox)
    {
        // Initialize //
        this.Hitbox = Hitbox;
        this.Sprite = Sprite;
        // Add Reference //
        Structures.Add(this);
    }

    [SetsRequiredMembers]
    public Structure(Bitmap? Sprite, Point Position, Size Size) : this(Sprite, new Rect(Position, Size)) {}
    // Event Methods //
    public override void OnRender(GuiService guiService, DrawingContext context)
    {
        // Conditions //
        if (WorldService.CurrentCamera is null)
            return;
        // Camera Transform //
        PushTransform(context, WorldService.CurrentCamera.OffsetMatrix);
        // Draw //
        if (Sprite is not null)
            context.DrawImage(Sprite, Hitbox);

        else context.DrawRectangle(Brushes.White, null, Hitbox);
        // Reset Matrix //
        PopMatrix();
    }
    // Disposal Methods //
    public override void Dispose()
    {
        Structures.Remove(this);
        base.Dispose();
    }
}