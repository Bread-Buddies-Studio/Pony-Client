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
using System.Net.Http.Headers;
using Rendering.Entities;
using Core.Interfaces;
using System.Diagnostics;

namespace Rendering.Visible;

public abstract class Renderable : IPhysical, IDisposable, IComparable<Renderable>
{
    // Static Data //
    public static SortedSet<Renderable> Renderables {
        get;
        private set;
    } = [];
    private readonly HashSet<DrawingContext.PushedState> Matrixes = [];
    // Identifier //
    public Guid UniqueIdentifier = Guid.NewGuid();
    // Hitbox //
    public virtual Rect Hitbox { get; set; }
    public virtual Point Position
    {
        get => Hitbox.Position;
        set => Hitbox = new(value, Size);
    }
    public virtual Size Size
    {
        get => Hitbox.Size;
        set => Hitbox = new(Position, value);
    }
    // Render Data //
    public virtual bool IsAlwaysOnTop { get; set; } = false; // If true, this will be rendered on top of other renderables
    public virtual bool AlwaysShows { get; set; } = false; // If true, this will always be rendered, even if not within sight
    public bool IsVisible { get; set; } = true;
    public byte Layer { get; set; }
    /// <summary> Position Based Layering </summary>
    public bool PBL = false;
    // Constructor //
    static Renderable() => RunService.Update += OnUpdateStatic;
    public Renderable() : base() => Renderables.Add(this);
    public Renderable(Rect Hitbox) : this() => this.Hitbox = Hitbox;
    // Base Methods //
    public bool IsSeenByLocalPlayer() => WorldService.CurrentCamera?.IsWithinSight(this) ?? false;
    // Matrix //
    public DrawingContext.PushedState PushTransform(DrawingContext context, Matrix transform)
    {
        DrawingContext.PushedState pushedState = context.PushTransform(transform);
        // Add to Matrix List //
        Matrixes.Add(pushedState);
        // Return Action //
        return pushedState;
    }
    public DrawingContext.PushedState[] PushTransform(DrawingContext context, Matrix[] transforms)
    {
        // Init Array //
        DrawingContext.PushedState[] pushedStates = new DrawingContext.PushedState[transforms.Length];
        // Add Matrixes //
        foreach ((int index, Matrix transform) in transforms.Index())
            Matrixes.Add(pushedStates[index] = context.PushTransform(transform));
        // Return Array //
        return pushedStates;
    }
    public DrawingContext.PushedState PushOpacity(DrawingContext context, double opacity)
    {
        DrawingContext.PushedState pushedState = context.PushOpacity(opacity);
        // Add to Matrix List //
        Matrixes.Add(pushedState);
        // Return Action //
        return pushedState;
    }
    public void PopMatrix()
    {
        foreach (var matrix in Matrixes.Reverse())
            matrix.Dispose();
        
        Matrixes.Clear();
    }
    public void PopMatrix(DrawingContext.PushedState[] pushedStates)
    {
        // Start from last Matrix //
        pushedStates.Reverse();
        // Dispose Matrixes //
        foreach (var matrix in pushedStates)
        {
            // Conditions //
            if (!Matrixes.Contains(matrix))
                continue;
            // Delete //
            matrix.Dispose();
            // Remove Matrix //
            Matrixes.Remove(matrix);
        }
    }
    // Event Methods //
    private static void OnUpdateStatic(object? sender, UpdateEventArgs eventData)
    {
        // Get Camera //
        LocalPlayer? localPlayer = WorldService.LocalPlayer;
        Camera? localCamera = WorldService.LocalPlayer?.AttachedCamera;
        // Conditions //
        if (localPlayer is null)
            return;
        if (localCamera is null)
            return;
        // Update //
        foreach (var renderable in Renderables.Reverse<Renderable>())
        {
            // Conditions //
            if (!renderable.PBL)
                continue;
            if (!localCamera.IsWithinSight(renderable))
                continue;
            // Render //
            Renderables.UpdateElement(renderable);
        }
        //Renderables.Update();
    }
    public abstract void OnRender(GuiService guiService, DrawingContext context);
    // Deletion Methods //
    /// <summary>
    /// Removes this <see cref="Renderable"/> from the <see cref="Renderables"/> list
    /// </summary>
    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
        Renderables.Remove(this);
    }
    // Ancestor Methods //
    public int CompareTo(Renderable? renderable)
    {
        // Pre-Conditions //
        if (renderable is null)
            return -1;
        if (UniqueIdentifier == renderable.UniqueIdentifier)
            return 0;

        // Always On Top Conditions //
        if (IsAlwaysOnTop)
            return 1;
        if (renderable.IsAlwaysOnTop)
            return -1;

        // PBL //
        if (PBL)
            return renderable.Hitbox.Bottom > Hitbox.Bottom ? -1 : 1;
        else if (renderable.PBL)
            return renderable.Hitbox.Bottom < Hitbox.Bottom ? -1 : 1;

        // Post-Conditions //
        if (renderable.Layer == Layer) // Now depends on load in time //
            return -1;
        
        // Return //
        return renderable.Layer < Layer ? -1 : 1;
    }
}
