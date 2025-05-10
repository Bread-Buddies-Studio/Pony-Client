using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia;
using Core.Services;
using Rendering.Services;
using Rendering.Views;
using Core.Extensions;

namespace Rendering.Visible;

public abstract class Renderable : IComparable<Renderable>
{
    // Static Data //
    public static SortedSet<Renderable> Renderables
    {
        get;
        private set;
    } = [];
    private readonly List<DrawingContext.PushedState> Matrixes = [];
    // Identifier //
    public readonly Guid UniqueIdentifier = Guid.NewGuid();
    // Hitbox //
    public Rect Hitbox { get; set; }
    public Point Position
    {
        get => Hitbox.Position;
        set => Hitbox = new(value, Size);
    }
    public Size Size
    {
        get => Hitbox.Size;
        set => Hitbox = new(Position, value);
    }
    // Render Data //
    private byte _layer;
    public byte Layer
    {
        get => _layer;

        set
        {
            // Settings //
            _layer = value;
            // Update //
            Renderables.UpdateElement(this);
        }
    }
    /// <summary> Position Based Layering </summary>
    public bool PBL = false;
    // Constructor //
    static Renderable() => RunService.Update += StaticUpdate;
    public Renderable() : base() => Renderables.Add(this);
    public Renderable(Rect Hitbox) : this()
    {
        // Initialize //
        this.Hitbox = Hitbox;
    }
    // Base Methods //
    public void PushTransform(DrawingContext context, Matrix transform)
    {
        Matrixes.Insert(0, context.PushTransform(transform));
    }
    public void PopMatrix()
    {
        foreach (var matrix in Matrixes)
            matrix.Dispose();

        Matrixes.Clear();
    }
    // Event Methods //
    private static void StaticUpdate(object? sender, UpdateEventArgs eventData)
    {
        foreach (var renderable in Renderables.Reverse())
        {
            // Conditions //
            if (!renderable.PBL)
                continue;
            // Update Ordering //
            Renderables.UpdateElement(renderable);
        }
    }
    public abstract void OnRender(GuiService guiService, DrawingContext context);
    // Deletion Methods //
    /// <summary>
    /// Removes this <see cref="Renderable"/> from the <see cref="Renderables"/> list
    /// </summary>
    public virtual void Delete() => Renderables.Remove(this);
    // Ancestor Methods //
    public int CompareTo(Renderable? renderable)
    {
        // Pre-Conditions //
        if (renderable is null)
            return -1;
        if (UniqueIdentifier == renderable.UniqueIdentifier)
            return 0;

        // PBL //
        if (PBL && renderable.PBL)
            return renderable.Hitbox.Bottom > Hitbox.Bottom ? -1 : 1;

        // Post-Conditions //
        if (renderable.Layer == Layer) // Now depends on load in time //
            return -1;

        // Return //
        return renderable.Layer < Layer ? -1 : 1;
    }
}
