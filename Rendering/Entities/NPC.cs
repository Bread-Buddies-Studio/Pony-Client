using Avalonia;
using Avalonia.Dialogs.Internal;
using Avalonia.Media;
using Core.Extensions;
using Rendering.Entities.Accessories;
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
    public override float DesiredSpeed { get; set; } = 5;

    // Prompt Data //
    private readonly InteractPrompt interactPrompt;
    // Events //
    public event EventHandler<EventArgs>? ReachedTarget;
    // Constructor //
    [SetsRequiredMembers]
    public NPC(string Name, Point Position, Size Size, params string[] accessoryNames) : base(Position, Size, accessoryNames) 
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
        float distance = (float)Position.DistanceFrom((Point)TargetPosition);
        Point direction = Position.ToDirectional((Point)TargetPosition).SetMagnitude(Speed.Clamp0(distance));
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
    private void UpdateState() => State = Moving ? EntityState.Moving : EntityState.Idle;
    private void UpdateAnimation()
    {
        // Update Animation //
        Point facingDirection = FacingDirection.ToPoint();
        // Checks //
        facingDirection = facingDirection.WithX(Math.Abs(facingDirection.X));
        // Play Animation //
        AnimController.Play(Name.ToLower(), State.ToString(), (int)facingDirection.ToDegrees());
    }
    #endregion
    #region Event Methods
    public void OnInteract(object? sender, EventArgs eventData)
    {
        // Convert //
        LocalPlayer? player = sender as LocalPlayer;
        // Conditions //
        if (player is null) // How?
            return;
        // Init Message //
        string message = "[sine=1 10]Wore [\\sine]";
        // Settings //
        foreach ((int index, Accessory accessory) in this.Accessories.Index())
        {
            // Padding //
            if (index is not 0)
                message += ", ";
            // Print Accessory Name //
            message += accessory.Name;
            // Wear //
            player.Accessories.Add(new Accessory(accessory.Name));
        }

        _ = DialogueService.Typewrite(message);

        //_ = DialogueService.Typewrite(
        //        string.Concat([
        //            $"[size=30][b]{Name}[\\b][\\size]\n ",
        //            "Hello... I've got my [sine=1 10][nervous=1][b][color=red]eyes[\\color][\\b][\\nervous][\\sine] on [color=red]you[\\color]"
        //        ]));
    }
    public override void OnUpdate(long deltaTime)
    {
        if (WorldService.LocalPlayer?.Position != TargetPosition)
            TargetPosition = WorldService.LocalPlayer?.Position;

        UpdateMoveDirection();
        UpdateState();

        if (interactPrompt.IsVisible && WorldService.LocalPlayer is not null)
        {
            // Update Facing Direction //
            FacingDirection = Hitbox.Center
                .ToDirectional(WorldService.LocalPlayer.Hitbox.Center)
                .ToDegrees();
            // Update Animation for Degrees //
            UpdateAnimation();
        }
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
    #endregion
}