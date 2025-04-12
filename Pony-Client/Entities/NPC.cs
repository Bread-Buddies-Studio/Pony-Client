using Avalonia;
using Avalonia.Media;
using LiteNetLib.Utils;
using Pony_Client.Extensions;
using Pony_Client.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pony_Client.Entities;

public class NPC : Entity
{
    public Point? TargetPosition;
    public override Point MovementDirection { get; set; }
    public override float Speed { get; set; }
    // Constructor //
    [SetsRequiredMembers]
    public NPC(string Name, Point Position, Size Size) : base(Position, Size) 
    {
        _ = AnimController.Play(State.ToString().ToLower());
        this.Name = Name;
    }
    #region Network Methods
    public override void Serialize(NetDataWriter writer)
    {
        throw new NotImplementedException();
    }
    public override void Deserialize(NetDataReader reader)
    {
        throw new NotImplementedException();
    }
    #endregion
    #region Event Methods
    public override void OnRender(GuiService guiService, DrawingContext context)
    {
        IImage? frame = AnimController.Frame;
        // Condtitions //
        if (AttachedCamera is null)
        {
            AttachedCamera = WorldService.localPlayer?.AttachedCamera;
            return;
        }
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
        _ = AnimController.Play(state.newState.ToString());
    }

    public override void OnUpdate(long deltaTime)
    {
        // Conditions //
        if (Position == TargetPosition)
            TargetPosition = null;
        if (TargetPosition is null)
            return;
        // Calculations //
        Point direction = Position.ToDirectional((Point)TargetPosition);
        // Settings //
        MovementDirection = direction;
    }
    #endregion
}