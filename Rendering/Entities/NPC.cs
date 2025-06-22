using Avalonia;
using Avalonia.Media;
using Core.Extensions;
using LiteNetLib.Utils;
using Rendering.Services;
using Rendering.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rendering.Entities;

public class NPC : Entity
{
    public Point? TargetPosition;
    public override Point MovementDirection { get; set; }
    public override float Speed { get; set; }
    // Events //
    public event EventHandler<EventArgs>? ReachedTarget;
    // Constructor //
    [SetsRequiredMembers]
    public NPC(string Name, Point Position, Size Size) : base(Position, Size) 
    {
        this.Name = Name;
        AnimController.Play(State.ToString().ToLower());
    }
    #region Base Methods
    public void UpdateMoveDirection()
    {
        // Checks //
        if (Position == TargetPosition)
            TargetPosition = null;
        // Conditions //
        if (TargetPosition is null)
            return;
        // Calculations //
        Point direction = Position.ToDirectional((Point)TargetPosition);
        // Settings //
        MovementDirection = direction;
    }
    #endregion
    #region Event Methods
    public override void OnUpdate(long deltaTime)
    {
        UpdateMoveDirection();
        // Move //
        base.OnUpdate(deltaTime);
    }
    public override void OnRender(GuiService guiService, DrawingContext context)
    {
        IImage? frame = AnimController.Frame;
        // Condtitions //
        if (WorldService.CurrentCamera is null)
            return;
        if (frame is null)
            return;
        // Data //
        Point position = Position;
        // Transformations //
        if (AttachedCamera is not null)
            PushTransform(context, AttachedCamera.OffsetMatrix);
        if (!Direction)
        {
            position = new Point(-position.X, position.Y);
            PushTransform(context, new Matrix(-1, 0, 0, 1, Size.Width, 0));
        }
        // Draw Image //
        context.DrawImage(frame, new(position.X, position.Y, Size.Width, Size.Height));
        // Reset Matrix //
        PopMatrix();
    }

    public override void OnStateChange(object? entity, (EntityState oldState, EntityState newState) state)
    {
        // Play Animation //
        AnimController.Play(state.newState.ToString());
    }
    #endregion
}