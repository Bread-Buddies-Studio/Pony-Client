using Avalonia;
using Core.Extensions;
using Core.Services;
using LiteNetLib.Utils;
using LiteNetLib;
using Network;
using Network.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Network.Enums;

namespace Rendering.Entities;

/// <summary>
/// A <see cref="Player"/> owned by the local client
/// </summary>
public class LocalPlayer : Player
{
    #region Constructors
    [SetsRequiredMembers]
    public LocalPlayer(string Name, Rect Hitbox) : base(Name, Hitbox)
    {
        // Initialize //
        this.AttachedCamera = new(this);
        this.Name = Name;
    }
    [SetsRequiredMembers]
    public LocalPlayer(string Name, Point Position, Size Size) : this(Name, new Rect(Position, Size)) {}
    #endregion
    #region Base Methods
    public override void Speak(string message)
    {
        // Add Log //
        base.Speak(message);
        // Send Network Message //
        PacketService.SendPacket(PacketType.Message, message);
    }
    public void UpdateMovement(double deltaTime)
    {
        // Set MovementDirection to the Controls // e.g Arrow Keys
        MovementDirection = new Point(
            InputService.GetAxis(InputAxis.Horizontal),
            InputService.GetAxis(InputAxis.Vertical))
            .SetMagnitude(Speed);
        // Set Magnitude to prevent faster diagonal speeds //
        MovementDirection = MovementDirection.SetMagnitude(Speed);
        // Facing Direction //
        if (InputService.GetAxis(InputAxis.Horizontal) is not 0)
            Direction = InputService.GetAxis(InputAxis.Horizontal) > 0;
    }
    #endregion
    #region Event Methods
    public override void OnUpdate(long deltaTime)
    {
        // Local Player Update //
        UpdateMovement(deltaTime);
        // Change State //
        State = Moving ? EntityState.Moving : EntityState.Idle;
        // Regular Update //
        base.OnUpdate(deltaTime);
        // Send Network Packet //
        PacketService.SendPacket(PacketType.PlayerUpdate, this);
    }
    #endregion
}