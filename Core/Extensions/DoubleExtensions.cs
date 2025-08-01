using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Avalonia;

namespace Core.Extensions;

public static partial class DoubleExtensions
{
    public static double Clamp(this double number, double min, double max) => Math.Clamp(number, min, max);
    public static double Clamp01(this double number) => number.Clamp(0, 1);

    public static double ToRadians(this double degrees) => double.DegreesToRadians(degrees);
    public static double ToDegrees(this double radians) => double.RadiansToDegrees(radians);

    public static Point ToPoint(this double degrees) => new (Math.Cos(degrees.ToRadians()), Math.Sin(degrees.ToRadians()));
}