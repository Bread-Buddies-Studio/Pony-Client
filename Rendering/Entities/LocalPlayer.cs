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
using System.ComponentModel.Design;

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
    }
    #endregion
    #region Event Methods
    public override void OnUpdate(long deltaTime)
    {
        if (Moving)
        {
            if (InputService.IsKeyDown("fly"))
                State = EntityState.Flying;
            else if (InputService.IsKeyDown("run"))
                State = EntityState.Running;
            else
                State = EntityState.Moving;
        }

        else State = EntityState.Idle;
        // Local Player Update //
        UpdateMovement(deltaTime);
        // Regular Update //
        base.OnUpdate(deltaTime);
        // Send Network Packet //
        PacketService.SendPacket(PacketType.PlayerUpdate, this);
    }
    #endregion
}