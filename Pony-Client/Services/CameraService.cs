using Avalonia;
using Avalonia.Media;
using Pony_Client.Entities;
using Pony_Client.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pony_Client.Services;

public static class CameraService
{
   
}

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