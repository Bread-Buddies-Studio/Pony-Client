using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Core.Extensions;
using Core.Services;
using Rendering.Services;
using Rendering.Views;
using Rendering.Visible;

namespace Rendering.Entities.Accessories;

public class Accessory : Renderable
{
    // Data //
    public required Entity Parent;
    public required string Name;
    public Animator Animator = new();
    public override Rect Hitbox => Parent is null ? new() : Parent.Hitbox;
    public override byte Layer
    {
        get
        {
            // Conditions //
            if (Parent is null)
                return 0;
            // Successful Calculation //
            return (byte)(Parent.Layer + Animator.Layer ?? 1);
        }
    }
    // Constructor //
    [SetsRequiredMembers]
    public Accessory(Entity Parent, string Name) : base()
    {
        // Init //
        this.Parent = Parent;
        this.Layer = (byte)(Parent.Layer + 1); // Accessories are always one layer above the parent entity
        this.Name = Name;
        this.PBL = true;
        // Add Reference //
        Parent.Accessories.Add(this);
        // Play //
        RunService.Update += OnUpdate;
    }


    // Event Methods //
    private void UpdateAnimation()
    {
        // Update Animation //
        Point facingDirection = Parent.FacingDirection.ToPoint();
        // Checks //
        facingDirection = facingDirection.WithX(Math.Abs(facingDirection.X));
        // Play Animation //
        Animator.Play(Name.ToLower(), Parent.State.ToString(), (int)facingDirection.ToDegrees());
    }
    private void OnUpdate(object? sender, UpdateEventArgs _)
    {
        //Debug.WriteLine("Update!");
        UpdateAnimation();
    }
    public override void OnRender(GuiService guiService, DrawingContext context)
    {
        IImage? frame = Animator.Frame;
        // Condtitions //
        if (frame is null)
            return;
        if (WorldService.CurrentCamera is null)
            return;
        // Transformations //
        PushTransform(context, Parent.RenderMatrix); // Camera Offset //
        // Draw Image //
        context.DrawImage(frame, new(0, 0, Size.Width, Size.Height));
        // Reset Matrix //
        PopMatrix();
    }
}
