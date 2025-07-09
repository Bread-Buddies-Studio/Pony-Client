// Directory //
namespace Rendering.Entities;
// Imports //
using LiteNetLib.Utils;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Avalonia;
using Avalonia.Controls.Platform;
using Avalonia.Media;
using System.Diagnostics;
using System;
using System.Linq;
using Rendering.Visible;
using Core.Services;
using Rendering.Services;
using Rendering.Views;
using Core.Extensions;
using Network.Services;
using Network;
using LiteNetLib;
using Core.Structs;
using Core.Interfaces;

// Enumerations //
public enum EntityState
{
    Idle,
    Moving,
}
// Classes //
public abstract partial class Entity : Renderable, ICollidable, INetSerializable
{
    // Static Data //
    public static readonly HashSet<Entity> Entities = [];
    // Instance Data //
    public required string Name = string.Empty;
    public readonly List<OverheadText> OverheadTexts = [];
    public Camera? AttachedCamera;
    public readonly List<DrawingContext.PushedState> Matrixes = [];
    // Dialogue //
    public Dictionary<char, int> CharacterDelays = new()
    { 
        {' ', 50} 
    };
    // Movement //
    public EntityState State
    {
        get => field;

        internal set
        {
            // Conditions //
            if (field == value)
                return;
            // Invoke Event //
            StateChanged?.Invoke(this, (field, value));
            // Settings //
            field = value;
        }
    } = EntityState.Idle;
    public bool Moving => MovementDirection != new Point();
    public abstract Point MovementDirection { get; set; }
    public abstract float Speed { get; set; }
    public bool Direction { get; set; }
    // ICollidable Data //
    public required ICollidable CollManager { get; init; }
    public bool IsCollidable { get; set; }
    public bool IsAnchored { get; set; }

    // Events //
    public event EventHandler<(EntityState, EntityState)>? StateChanged;
    // Constructors //
    [SetsRequiredMembers]
    public Entity(Rect Hitbox) : base(Hitbox)
    {
        // Settings //
        this.PBL = true;
        this.Hitbox = Hitbox;
        this.CollManager = this;
        // Add Reference //
        Entities.Add(this);
        // Connections //
        RunService.Update += OnUpdate;
        StateChanged += OnStateChange;
        // Initialize State //
        StateChanged?.Invoke(this, (State, State));
    }

    [SetsRequiredMembers]
    public Entity(Point Position, Size Size) : this(new Rect())
    {
        this.Position = Position;
        this.Size = Size;
    }
    // Movement Methods //
    public Point MoveTo(Point Position)
    {
        // Settings //
        this.Position = Position;
        // Return New Position //
        return Position;
    }
    public Point MoveTo(float x, float y) => MoveTo(new(x, y));
    public Point MoveTo(double x, double y) => MoveTo(new(x, y));
    public Point Translate(Point Offset)
    {
        // Settings //
        Position += Offset;
        // Return New Position //
        return Position;
    }
    public Point Translate(float x, float y) => Translate(new(x, y));
    public Point Translate(double x, double y) => Translate(new(x, y));
    public virtual void Speak(string message) => ChatService.AddLog(this, message);
    #region Network Methods
    public virtual void Serialize(NetDataWriter writer)
    {
        writer.Put(Name);
        writer.Put((byte)State);
        writer.Put(Direction);
        writer.PutArray(Position.ToDoubleArray());
        writer.PutArray(Size.ToDoubleArray());
    }
    public virtual void Deserialize(NetDataReader reader)
    {
        Name = reader.GetString();
        State = (EntityState)reader.GetByte();
        Direction = reader.GetBool();
        Position = PointExtensions.FromDoubleArray(reader.GetDoubleArray());
        Size = SizeExtensions.FromDoubleArray(reader.GetDoubleArray());
    }
    #endregion
    #region Event Methods
    private void OnUpdate(object? sender, UpdateEventArgs eventData) => OnUpdate(eventData.DeltaTime);
    public virtual void OnUpdate(long deltaTime)
    {
        // Conditions //
        if (deltaTime is 0)
            return;
        // Movement //
        Translate(MovementDirection / 20 * deltaTime);
    }
    public abstract void OnStateChange(object? entity, (EntityState oldState, EntityState newState) state);
    #endregion
}
// Animation //
public abstract partial class Entity
{
    // Controller //
    public readonly Animator AnimController = new()
    {
        Looped = true
    };
    // Event Methods //
    public override void OnRender(GuiService guiservice, DrawingContext context)
    {
        IImage? frame = AnimController.Frame;
        // Condtitions //
        if (frame is null)
            return;
        if (AttachedCamera is null)
            return;
        // Data //
        Point position = Position;
        // Transformations //
        PushTransform(context, AttachedCamera.OffsetMatrix); // Camera Offset //
        if (!Direction) // Direction Offset //
        {
            position = new Point(-position.X, position.Y);
            PushTransform(context, new Matrix(-1, 0, 0, 1, Size.Width, 0));
        }
        // Draw Image //
        context.DrawImage(frame, new(position.X, position.Y, Size.Width, Size.Height));
        // Reset Matrix //
        PopMatrix();
    }
}
// Other //
