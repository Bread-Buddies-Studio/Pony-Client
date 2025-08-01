using Avalonia;
using Avalonia.Dialogs.Internal;
using Avalonia.Media;
using Core.Extensions; 
using Rendering.Services;
using Rendering.Views;
using Rendering.Visible;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rendering.Entities;

public class NPC : Entity
{
    // Public Data //
    public Point? TargetPosition;
    public required double InteractRange = 200;
    public readonly NameTag NameTag;
    // Ancestor Data //
    public override Point MovementDirection { get; set; }
    public override float Speed { get; set; }
    // Prompt Data //
    private readonly Point promptOffset = new(0, -5);
    private readonly InteractPrompt interactPrompt;
    // Events //
    public event EventHandler<EventArgs>? ReachedTarget;
    // Constructor //
    [SetsRequiredMembers]
    public NPC(string Name, Point Position, Size Size) : base(Position, Size) 
    {
        // Initialize //
        this.Name = Name;
        this.NameTag = new(this);
        this.interactPrompt = new(this);
        // Play Animation //
        OnMoveDirectionChange(this, (new(), new()));
        // Connections //
        interactPrompt.Interact += OnInteract;
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
    #region Private Methods
    private Player[] ScanForPlayers(double scanningRange)
    {
        return [.. Player.Players.Where(player => this.Position.DistanceFrom(player.Position) <= scanningRange)];
    }
    private Player? GetClosestPlayer()
    {
        return Player.Players.MinBy(player => Position.DistanceFrom(player.Position));
    }
    private void UpdateAnimation()
    {
        // Update Animation //
        Point facingDirection = FacingDirection.ToPoint();
        // Checks //
        facingDirection = facingDirection.WithX(Math.Abs(facingDirection.X));
        // Play Animation //
        AnimController.Play(Name.ToLower(), (int)facingDirection.ToDegrees(), State.ToString());
    }
    #endregion
    #region Event Methods
    public void OnInteract(object? sender, EventArgs eventData)
    {
        _ = DialogueService.Typewrite(
                string.Concat([
                    $"[size=30][b]{Name}[\\b][\\size]\n ",
                    "Hello... I've got my [sine=1 10][nervous=1][b][color=red]eyes[\\color][\\b][\\nervous][\\sine] on [color=red]you[\\color]"
                ]));
    }
    public override void OnUpdate(long deltaTime)
    {
        if (interactPrompt.IsVisible && WorldService.LocalPlayer is not null)
        {
            // Update Facing Direction //
            FacingDirection = 
                Hitbox.Center.ToDirectional(WorldService.LocalPlayer.Hitbox.Center)
                .ToDegrees();
            // Update Animation for Degrees //
            UpdateAnimation();
        }
        UpdateMoveDirection();
        // Move //
        base.OnUpdate(deltaTime);
    }
    public override void OnStateChange(object? entity, (EntityState oldState, EntityState newState) state)
    {
    }
    public override void OnMoveDirectionChange(object? entity, (Point oldDirection, Point newDirection) direction)
    {
        UpdateAnimation();
        // Activate Ancestor Method //
        base.OnMoveDirectionChange(entity, direction);
    }
    public override void OnRender(GuiService guiservice, DrawingContext context)
    {
        IImage? frame = AnimController.Frame;
        // Condtitions //
        if (frame is null)
            return;
        if (WorldService.CurrentCamera is null)
            return;
        // Data //
        Point position = Position;
        // Transformations //
        PushTransform(context, WorldService.CurrentCamera.OffsetMatrix); // Camera Offset //
        if (FacingDirection is < -90 or > 90) // Direction Offset //
        {
            position = new Point(-position.X, position.Y);
            PushTransform(context, new Matrix(-1, 0, 0, 1, Size.Width, 0));
        }
        // Draw Image //
        context.DrawImage(frame, new(position.X, position.Y, Size.Width, Size.Height));
        // Reset Matrix //
        PopMatrix();
    }
    #endregion
}