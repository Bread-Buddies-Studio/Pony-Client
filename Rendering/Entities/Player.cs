// Directory //
namespace Rendering.Entities;
// Imports //
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using Avalonia;
using Network.Services;
using LiteNetLib;
using Rendering.Visible;
using System.Linq;
using Network.Enums;
using Core.Extensions;
using System.Diagnostics;
using Rendering.Services;
using LiteNetLib.Utils;
using Network.Extensions;
using System;
using Core.Interfaces;
using Tmds.DBus.Protocol;
using System.Threading.Tasks;
using Rendering.Views;
using Avalonia.Media;
using Avalonia.Media.Imaging;

// Class //
public class Player : Entity, IStats
{
    // Static Data //
    public readonly static HashSet<Player> Players = [];
    // Required //
    public required byte TagLine;
    public required NameTag NameTag;
    // Vitality //
    public int Health
    {
        get => field;
        set => field = value.Clamp0(MaxHealth);
    }

    public int Mana
    {
        get => field;
        set => field = value.Clamp0(MaxMana);
    }

    public int MaxHealth { get; set; }
    public int MaxMana { get; set; }
    // Movement //
    public override float Speed { get; set; } = 10;

    #region Constructors
    [SetsRequiredMembers]
    public Player(string Name, Rect Hitbox) : base(Hitbox)
    {
        this.Name = Name;
        this.NameTag = new(this);
        this.AttachedCamera = WorldService.LocalPlayer?.AttachedCamera;
        // Reference //
        Players.Add(this);
        // Play Animation //
        OnMoveDirectionChange(this, (new(), new()));
    }

    [SetsRequiredMembers]
    public Player(string Name, Point Position, Size Size) : this(Name, new Rect(Position, Size)) {}

    [SetsRequiredMembers]
    private Player() : this(string.Empty, new(200, 200), new(200, 200)) { }
    
    static Player() => NetworkService.NetworkReceive += OnNetworkReceive;
    #endregion
    // Private Methods //
    private void UpdateAnimation()
    {
        // Update Animation //
        Point facingDirection = FacingDirection.ToPoint();
        // Checks //
        facingDirection = facingDirection.WithX(Math.Abs(facingDirection.X));
        // Play Animation //
        AnimController.Play("redheart", (int)facingDirection.ToDegrees(), State.ToString());
    }
    // Static Methods //
    /// <summary>
    /// Finds a specified <see cref="Player"/> in the <see cref="Players"/> List.
    /// Creates a new <see cref="Player"/> if not found.
    /// </summary>
    /// <param name="accountData"></param>
    /// <returns></returns>
    public static Player FindFromAccountData(string accountData)
    {
        // Split Data //
        string[] data = accountData.Split('|', 2);
        // Extract Data //
        byte tagLine = byte.Parse(data[0]);
        string userName = data[1];
        // Search //
        return Players.FirstOrDefault(player => tagLine == player.TagLine && userName == player.Name)
            ?? new() 
            { 
                Name = userName,
                Position = new Point(200, 200),
                Size = new Size(200, 200)
            };
    }
    #region Event Methods
    private static void OnNetworkReceive(object? sender, (NetPeer server, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod) data)
    {
        // Pre-Conditions //
        if (data.reader.EndOfData)
            return;
        // Get Packet Type // 
        PacketType packetType = (PacketType)data.reader.PeekByte();
        // Conditions //
        if (packetType is not PacketType.PlayerUpdate)
            return;
        // Set Position //
        data.reader.SkipByte();
        // Extract Data //
        string accountData = data.reader.GetString();
        // Find Old Player //
        Player playerToUpdate = FindFromAccountData(accountData);
        // Extract New Player //
        data.reader.Get(() => playerToUpdate);
    }
    public override void OnUpdate(long deltaTime)
    {
        // Update //
        base.OnUpdate(deltaTime);
        // Update Animations //
        UpdateAnimation();
        // Conditions //
        if (!IsCollidable)
            return;
        // Collisions //
        foreach (var structure in Structure.Structures)
        {
            // Conditions //
            if (!CollManager.IsCollidingWith(structure))
                continue;
            // Collision Detected //
            CollManager.Seperate(structure);
        }
    }
    public override void OnStateChange(object? entity, (EntityState oldState, EntityState newState) state)
    {
        // Update Animation //
        //Debug.WriteLine("Playing Animation!");
        //AnimController.Play("mare", (int)GetDirection(), state.newState.ToString());
    }
    public override void OnMoveDirectionChange(object? entity, (Point oldDirection, Point newDirection) direction)
    {
    }
    #endregion
    // Disposal Methods //
    public override void Dispose()
    {
        Players.Remove(this);
        base.Dispose();
    }
}