// Directory //
namespace Pony_Client.Entities;
// Imports //
using LiteNetLib.Utils;
using Pony_Client.Services;
using Pony_Client.Views;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Avalonia;
using Avalonia.Controls.Platform;
using Avalonia.Media;
using System.Diagnostics;
using System;
using System.Linq;
// Enumerations //
public enum EntityState
{
    Idle,
    Moving,
}
// Classes //
public abstract partial class Entity : Renderable, INetSerializable
{
    // Static Data //
    public static readonly HashSet<Entity> Entities = [];
    // Instance Data //
    public required string Name = string.Empty;
    public readonly List<OverheadText> OverheadTexts = [];
    public Camera? AttachedCamera;
    public readonly List<DrawingContext.PushedState> Matrixes = [];
    // Movement //
    private EntityState _state = EntityState.Idle;
    public EntityState State { 
        get => _state;

        private set
        {
            // Conditions //
            if (_state == value)
                return;
            // Invoke Event //
            StateChanged?.Invoke(this, (_state, value));
            // Settings //
            _state = value;
        }
    }
    public bool Moving => MovementDirection != new Point();
    public abstract Point MovementDirection { get; set; }
    public abstract float Speed { get; set; }
    public bool Direction { get; set; }
    // Events //
    public event EventHandler<(EntityState, EntityState)>? StateChanged;
    // Constructors //
    [SetsRequiredMembers]
    public Entity(Rect Hitbox) : base(Hitbox)
    {
        // Settings //
        this.PBL = true;
        this.Hitbox = Hitbox;
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
    public void Speak(string message)
    {
        ChatService.AddLog(this, message);
    }
    #region Network Methods
    public abstract void Serialize(NetDataWriter writer);
    public abstract void Deserialize(NetDataReader reader);
    #endregion
    #region Event Methods
    private void OnUpdate(object? sender, UpdateEventArgs eventData)
    {
        // Extract Data //
        long deltaTime = eventData.DeltaTime;
        // Conditions //
        if (deltaTime is 0)
            return;
        // Movement //
        Translate(MovementDirection / 20 * deltaTime);
        // Change State //
        State = Moving ? EntityState.Moving : EntityState.Idle;
        // Call Real //
        OnUpdate(eventData.DeltaTime);
    }
    public abstract void OnUpdate(long deltaTime);
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
}
// Other //
