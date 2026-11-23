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
using Core.Interfaces;
using Avalonia.Media.Imaging;
using Avalonia.Automation.Provider;
using Rendering.Entities.Accessories;

// Enumerations //
public enum EntityState
{
    Idle,
    Moving,
    Running,
    Flying,
}

// Classes //
public abstract class Entity : Renderable, ICollidable, INetSerializable
{
    // Static Data //
    public static readonly HashSet<Entity> Entities = [];
    // Instance Data //
    public required string Name = string.Empty;
    // Arrays //
    public readonly List<OverheadText> OverheadTexts = [];
    public readonly HashSet<Accessory> Accessories = [];
    // Rendering //
    public readonly Animator AnimController = new();
    public override byte Layer => 1;
    public Matrix RenderMatrix
    {
        get
        {
            // Condtitions //
            if (AnimController.IsPlaying is false)
                return Matrix.Identity;
            if (WorldService.CurrentCamera is null)
                return Matrix.Identity;
            // Data //
            Matrix matrix = WorldService.CurrentCamera.OffsetMatrix;
            // Transformations //
            if (FacingDirection is < -90 or > 90) // Direction Offset //
                matrix = matrix.Prepend(Matrix.CreateScale(-1, 1))
                    .Append(Matrix.CreateTranslation(Size.Width, 0));
            // Add Position //
            matrix = matrix.Append(Matrix.CreateTranslation(Position));
            // Return //
            return matrix;
        }
    }
    // Dialogue //
    public Dictionary<char, int> CharacterDelays = new()
    { 
        {' ', 50},
        {'.', 100 }
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
            var oldValue = field;
            // Settings //
            field = value;
            // Invoke Event //
            StateChanged?.Invoke(this, (oldValue, value));
        }
    } = EntityState.Idle;
    public bool Moving => MovementDirection != new Point();
    public virtual Point MovementDirection
    { 
        get;

        set
        {
            // Conditions //
            if (field == value)
                return;
            // Data //
            var oldValue = field;
            // Settings //
            field = value;
            // Checks //
            if (Moving)
                FacingDirection = Direction;
            // Invoke Changed Event //
            MoveDirectionChanged?.Invoke(this, (oldDirection: oldValue, newDirection: value));
        }
    }
    public abstract float DesiredSpeed { get; set; }
    public float Speed { get; private set; }
    public double Direction => MovementDirection.ToDegrees();
    public double FacingDirection;
    // ICollidable Data //
    public required ICollidable CollManager { get; init; }
    public bool IsCollidable { get; set; }
    public bool IsAnchored { get; set; }

    // Events //
    public event EventHandler<(EntityState, EntityState)>? StateChanged;
    public event EventHandler<(Point oldDirection, Point newDirection)>? MoveDirectionChanged;
    // Constructors //
    [SetsRequiredMembers]
    public Entity(Rect Hitbox, params string[] accessoryNames) : base(Hitbox)
    {
        // Settings //
        this.PBL = true;
        this.Layer = 0;
        this.Hitbox = Hitbox;
        this.CollManager = this;
        
        // Add Reference //
        Entities.Add(this);

        // Connections //
        RunService.Update += OnUpdate;
        StateChanged += OnStateChange;
        MoveDirectionChanged += OnMoveDirectionChange;

        // Add Accessories //
        foreach (string accessoryName in accessoryNames)
            Wear(accessoryName);

        // Initialize State //
        StateChanged?.Invoke(this, (State, State));
    }

    [SetsRequiredMembers]
    public Entity(Point Position, Size Size, params string[] accessoryNames) : this(new Rect(), accessoryNames)
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
    public void Wear(string accessory_name)
    {
        Accessory accessory = new(accessory_name) { Parent = this };
        Accessories.Add(accessory);
    }
    #region Network Methods
    public virtual void Serialize(NetDataWriter writer)
    {
        writer.Put(Name);
        writer.Put((byte)State);
        writer.Put(FacingDirection);
        writer.PutArray(Position.ToDoubleArray());
        writer.PutArray(Size.ToDoubleArray());
    }
    public virtual void Deserialize(NetDataReader reader)
    {
        Name = reader.GetString();
        State = (EntityState)reader.GetByte();
        FacingDirection = reader.GetDouble();
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
        // Handle Speed //
        Speed = Speed.Lerp(State is EntityState.Idle ? 0 : DesiredSpeed, .1f);
        // Offset Character //
        Translate(MovementDirection.SetMagnitude(Speed) / 20 * deltaTime);
        // Debug //
        //Debug.WriteLine($"Speed: {Speed}\nState: {State}\nDesired Speed: {DesiredSpeed}\n");
    }
    public abstract void OnStateChange(object? entity, (EntityState oldState, EntityState newState) state);
    public virtual void OnMoveDirectionChange(object? entity, (Point oldDirection, Point newDirection) direction) {}
    public override void OnRender(GuiService guiservice, DrawingContext context)
    {
        IImage? frame = AnimController.Frame;
        // Condtitions //
        if (frame is null)
            return;
        if (WorldService.CurrentCamera is null)
            return;
        // Transformations //
        PushTransform(context, RenderMatrix); // Camera Offset //
        // Draw Image //
        context.DrawImage(frame, new(0, 0, Size.Width, Size.Height));
        // Reset Matrix //
        PopMatrix();
    }
    #endregion
}