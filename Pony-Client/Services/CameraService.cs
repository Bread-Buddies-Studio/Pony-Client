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
    /// <summary>
    /// Returns a point between <paramref name="p1"/> and <paramref name="p2"/>
    /// from <paramref name="alpha"/>. 0 is <paramref name="p1"/>, 1 is <paramref name="p2"/>
    /// </summary>
    /// <param name="p1">First Point</param>
    /// <param name="p2">Second Point</param>
    /// <param name="alpha">Distance travelled between points.</param>
    /// <returns></returns>
    private static Point Lerp(Point p1, Point p2, double alpha)
    {
        double xPos = (p2.X - p1.X) * alpha; // 2.5
        double yPos = (p2.Y - p1.Y) * alpha; // 0

        return new (p1.X + xPos, p1.Y + yPos);
    }
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