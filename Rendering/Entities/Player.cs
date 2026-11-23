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
using Rendering.Entities.Accessories;

// Class //
public class Player : Entity
{
    // Static Data //
    public readonly static HashSet<Player> Players = [];
    // Required //
    public required byte TagLine;
    public required NameTag NameTag;
    public override float DesiredSpeed { get; set; }
    #region Constructors
    [SetsRequiredMembers]
    public Player(string Name, Rect Hitbox, params string[] accessoryNames) : base(Hitbox, accessoryNames)
    {
        this.Name = Name;
        this.NameTag = new(this);
        // Reference //
        Players.Add(this);
        // Play Animation //
    }

    [SetsRequiredMembers]
    public Player(string Name, Point Position, Size Size) : this(Name, new Rect(Position, Size)) {}

    [SetsRequiredMembers]
    private Player() : this(string.Empty, new Point(200, 200), new Size(200, 200)) { }
    
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
        AnimController.Play("mare", State.ToString(), (int)facingDirection.ToDegrees());
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
        foreach (ICollidable structure in Structure.Structures)
        {
            // Conditions //
            if (!structure.IsCollidingWith(this))
                continue;
            // Collision Detected //
            CollManager.Seperate(structure);
        }
    }
    #endregion
    // Disposal Methods //
    public override void Dispose()
    {
        Players.Remove(this);
        base.Dispose();
    }

    public override void OnStateChange(object? entity, (EntityState oldState, EntityState newState) state)
    {
    }
}