using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Core.Extensions;

public static class PointExtensions
{
    /// <summary>
    /// Converts a double <see cref="Array"/> to a <see cref="Point"/>
    /// </summary>
    /// <param name="axis"></param>
    /// <returns></returns>
    public static Point FromDoubleArray(double[] axis) => new(axis[0], axis[1]);
    /// <summary>
    /// Converts a <see cref="Point"/> to a double <see cref="Array"/>
    /// </summary>
    /// <returns>The double array</returns>
    public static double[] ToDoubleArray(this Point position) => [position.X, position.Y];
    /// <summary>
    /// Gets the direction vector between 2 points
    /// </summary>
    /// <param name="p1">Origin</param>
    /// <param name="p2">Destination</param>
    /// <returns>Directional Vector</returns>
    public static Point ToDirectional(this Point p1, Point p2) => p2 - p1;
    /// <summary>
    /// Gets the directional vector between 2 points and normalises it
    /// </summary>
    /// <param name="p1">Origin</param>
    /// <param name="p2"></param>
    /// <returns></returns>
    public static Point ToDirectionalNormalised(this Point p1, Point p2) => p1.ToDirectional(p2).Normalize(); 
    /// <summary>
    /// Gets the length of a directional vector
    /// </summary>
    /// <param name="position">The directional vector</param>
    /// <returns>The length of the directional vector</returns>
    public static double GetMagnitude(this Point position)
    {
        // Conditions //
        if (position.X is 0 && position.Y is 0)
            return 0;
        else if (position.X is 0 && position.Y is not 0)
            return Math.Abs(position.Y);
        else if (position.Y is 0 && position.X is not 0)
            return Math.Abs(position.X);
        // If both axis aren't 0 //
        return Math.Sqrt(Math.Pow(position.X, 2) + Math.Pow(position.Y, 2));
    }
    /// <summary>
    /// Sets the length of a directional vector
    /// </summary>
    /// <param name="position">The directional vector</param>
    /// <param name="magnitude">The desired length of the directional vector</param>
    /// <returns>The updated directional vector</returns>
    public static Point SetMagnitude(this Point position, double magnitude)
    {
        // Sign //
        double sign;
        // Conditions //
        if (position.X is 0 && position.Y is 0)
            return position;
        else if (position.X is 0 && position.Y is not 0)
        {
            sign = Math.Abs(position.Y) / position.Y;
            position = new Point(0, magnitude * sign);
            return position;
        }
        else if (position.Y is 0 && position.X is not 0)
        {
            sign = Math.Abs(position.X) / position.X;
            position = new Point(magnitude * sign, 0);
            return position;
        }
        // If both axis aren't 0 //
        sign = Math.Abs(position.X) / position.X;
        double angle = Math.Atan(position.Y / position.X);
        double x = magnitude * Math.Cos(angle) * sign;
        double y = magnitude * Math.Sin(angle) * sign;

        position = new(x, y);
        // return new Position //
        return position;
    }
    /// <summary>
    /// Clamps Magnitude between 2 values.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns><see cref="Point"/> with clamped magnitude.</returns>
    public static Point ClampMagnitude(this Point position, double min, double max)
    {
        // Calculate Magnitude //
        double magnitude = position.GetMagnitude();
        // Checks Magnitude //
        if (magnitude < min) // Magnitude is less than min //
            return position.SetMagnitude(min);
        else if (magnitude > max) // Magnitude is greater than max //
            return position.SetMagnitude(max);
        // Do Nothing //
        return position;
    }
    /// <summary>
    /// Clamps Magnitude between 0 and 1.
    /// </summary>
    /// <param name="position"></param>
    /// <returns>The given <see cref="Point"/> with a magnitude between 0 and 1.</returns>
    public static Point Clamp01Magnitude(this Point position) => position.ClampMagnitude(0, 1);
    /// <summary>
    /// Sets the length of a directional vector to 1
    /// </summary>
    /// <param name="position">The directional vector</param>
    /// <returns></returns>
    public static Point Normalize(this Point position) => position / GetMagnitude(position);
    /// <summary>
    /// Returns a point between <paramref name="p1"/> and <paramref name="p2"/>
    /// from <paramref name="alpha"/>. 0 is <paramref name="p1"/>, 1 is <paramref name="p2"/>
    /// </summary>
    /// <param name="p1">First Point</param>
    /// <param name="p2">Second Point</param>
    /// <param name="alpha">Distance travelled between points.</param>
    /// <returns>The Interpolated <paramref name="p1"/></returns>
    public static Point Lerp(this Point p1, Point p2, double alpha)
    {
        double xPos = (p2.X - p1.X) * alpha; // 2.5
        double yPos = (p2.Y - p1.Y) * alpha; // 0

        return new(p1.X + xPos, p1.Y + yPos);
    }
    /// <summary>
    /// Converts a <see cref="Point"/> to a <see cref="Vector"/>
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public static Vector ToVector(this Point position) => (Vector)position;
    public static Point Translate(this Point position, Point offset) => position + offset;
    public static Point Translate(this Point position, double x, double y) => position.Translate(new(x, y));
    public static Point TranslateX(this Point position, double x) => position.Translate(x, 0);
    public static Point TranslateY(this Point position, double y) => position.Translate(0, y);
}
