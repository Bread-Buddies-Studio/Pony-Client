using Avalonia;
using Pony_Client.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pony_Client.Extensions;

namespace Pony_Client.Entities;

/// <summary>
/// The Player owned by the client
/// </summary>
public class LocalPlayer : Player
{
    #region Constructors
    [SetsRequiredMembers]
    public LocalPlayer(string Name, Rect Hitbox) : base(Name, Hitbox)
    {
        this.AttachedCamera = new(this);
        this.Name = Name;
    }
    [SetsRequiredMembers]
    public LocalPlayer(string Name, Point Position, Size Size) : this(Name, new Rect(Position, Size))
    {
    }
    #endregion
    #region Base Methods
    public void UpdateMovement(double deltaTime)
    {
        // Set MovementDirection to the Controls // e.g Arrow Keys
        MovementDirection = new(
            InputService.GetAxis(InputAxis.Horizontal),
            InputService.GetAxis(InputAxis.Vertical)
        );
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
        // Regular Update //
        base.OnUpdate(deltaTime);
        // Local Player Update //
        UpdateMovement(deltaTime);
        // Animations //
    }
    #endregion
}