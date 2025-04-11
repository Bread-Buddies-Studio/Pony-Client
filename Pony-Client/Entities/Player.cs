// Directory //
namespace Pony_Client.Entities;
// Imports //
using System.Numerics;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Pony_Client.Services;
using LiteNetLib.Utils;
using System.Collections.Generic;
using Avalonia.Media;
using Avalonia.Controls;
using System.Collections;
using Avalonia;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Pony_Client.Extensions;

// Class //
public class Player : Entity
{
    // Static Data //
    public readonly static HashSet<Player> Players = [];
    // Animation Controller //
    public readonly Animator animationController = new()
    {
        Looped = true
    };
    // Movement //
    public override Point MovementDirection { get; set; }
    public override float Speed { get; set; } = 10;
    #region Constructors
    [SetsRequiredMembers]
    public Player(string Name, Rect Hitbox) : base(Hitbox) 
    {
        _ = animationController.Play(State.ToString().ToLower());
        this.Name = Name;
    }

    [SetsRequiredMembers]
    public Player(string Name, Point Position, Size Size) : this(Name, new Rect(Position, Size))
    {
        this.Name = Name;
    }
    #endregion
    #region Base Methods

    #endregion
    #region Network Methods
    public override void Serialize(NetDataWriter writer)
    {
        writer.Put(false);
        // Unimplemented! //
        throw new NotImplementedException();
    }
    public override void Deserialize(NetDataReader reader)
    {
        // Unimplemented! //
        throw new NotImplementedException();
    }
    #endregion
    #region Event Methods
    public override void OnUpdate(long deltaTime)
    {

    }
    public override void OnRender(GuiService guiservice, DrawingContext context)
    {
        IImage? frame = animationController.Frame;
        // Condtitions //
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
        // Update Animation //
        _ = animationController.Play(state.newState.ToString().ToLower());
    }
    #endregion

    public static void main()
    {
        Point position = new();

        position.SetMagnitude(5);
    }
}