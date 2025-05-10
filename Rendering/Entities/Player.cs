// Directory //
namespace Rendering.Entities;
// Imports //
using System.Numerics;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using LiteNetLib.Utils;
using System.Collections.Generic;
using Avalonia.Media;
using Avalonia.Controls;
using System.Collections;
using Avalonia;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Rendering.Entities;

// Class //
public class Player : Entity
{
    // Static Data //
    public readonly static HashSet<Player> Players = [];
    // Movement //
    public override Point MovementDirection { get; set; }
    public override float Speed { get; set; } = 10;
    #region Constructors
    [SetsRequiredMembers]
    public Player(string Name, Rect Hitbox) : base(Hitbox) 
    {
        _ = AnimController.Play(State.ToString().ToLower());
        this.Name = Name;
        // Reference //
        Players.Add(this);
    }

    [SetsRequiredMembers]
    public Player(string Name, Point Position, Size Size) : this(Name, new Rect(Position, Size))
    {
        this.Name = Name;
    }
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
    public override void OnStateChange(object? entity, (EntityState oldState, EntityState newState) state)
    {
        // Update Animation //
        _ = AnimController.Play(state.newState.ToString().ToLower());
    }
    #endregion
    // Disposal Methods //
    public override void Delete()
    {
        Players.Remove(this);
        base.Delete();
    }
}