using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions;

public static class IntExtensions
{
    public static int Clamp(this int value, int min, int max) => Math.Clamp(value, min, max);
    public static int Clamp0(this int value, int max) => Math.Clamp(value, 0, max);
    public static int Clamp01(this int value) => Math.Clamp(value, 0, 1);
    public static int Distance(this int value, int other) => Math.Max(value, other) - Math.Min(value, other);


    public static int RoundToDegrees(this int value, int splitAmount)
    {
        // Calculations //
        int increment = 360 / splitAmount;
        int remainder = value % increment;
        // Checks //
        if (remainder < increment / 2)
            return value - remainder;
        else
            return value + (increment - remainder);
    }
}
