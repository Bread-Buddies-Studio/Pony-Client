using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia;
using Rendering.Entities;
using Core.Services;
using Rendering.Services;
using Core.Extensions;
using Rendering.Views;
using System.Diagnostics;

namespace Rendering.Visible;

public class Camera
{
    // Data //
    public Entity? Subject;
    public required Point Position;
    public double Speed = .05;
    public Matrix OffsetMatrix
    {
        get
        {
            return new Matrix(
                scaleX: 1,
                skewY: 0,
                skewX: 0,
                scaleY: 1,
                offsetX: -Position.X,
                offsetY: -Position.Y
            );
        }
    }
    // Constructor //
    [SetsRequiredMembers]
    public Camera(Entity Subject)
    {
        this.Subject = Subject;
        this.Position = Subject.Position;
        // Connections //
        RunService.Update += OnUpdate;
    }
    // Base Methods //
    public IEnumerable<Renderable> GetWithinSight() => Renderable.Renderables.Where(IsWithinSight);
    public IEnumerable<Renderable> GetWithinSight(Action<Renderable> action)
    {
        // Warning: Linq makes this slower, somebody please fix it...
        return Renderable.Renderables.Where(IsWithinSight);
    }
    public bool IsWithinSight(Rect otherObject)
    {
        // Conditions //
        if (GuiService.Window is null)
            return false;
        // Sight //
        Rect sight = new(Position.X, Position.Y, GuiService.Window.Width, GuiService.Window.Height);
        //
        return sight.Intersects(otherObject);
    }
    public bool IsWithinSight(Renderable otherObject) => IsWithinSight(otherObject.Hitbox);
    public void ApplyCamera(DrawingContext context) => Subject?.PushTransform(context, OffsetMatrix);
    // Conversion Methods //
    public Point GlobalToLocal(Point globalPosition)
    {
        return globalPosition.Transform(OffsetMatrix);
    }
    public Point LocalToGlobal(Point localPosition)
    {
        return localPosition.Transform(OffsetMatrix.Invert());
    }
    // Event Methods //
    public void OnUpdate(object? sender, UpdateEventArgs eventData)
    {
        // Conditions //
        if (Subject is null)
            return;
        if (GuiService.Window is null)
            return;
        // Settings //
        Position = Position.Lerp(Subject.Position
            - new Point(GuiService.Window.Width, GuiService.Window.Height) / 2
            + new Point(Subject.Size.Width, Subject.Size.Height) / 2,

            Speed / 20 * eventData.DeltaTime); // Interpolation Amount //
    }
}