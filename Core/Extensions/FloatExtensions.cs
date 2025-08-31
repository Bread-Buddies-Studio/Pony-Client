using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions;

public static class FloatExtensions
{
    public static float Clamp(this float number, float min, float max) => Math.Clamp(number, min, max);
    public static float Clamp0(this float number, float max) => number.Clamp(0, max);
    public static float Clamp01(this float number) => number.Clamp(0, 1);
}
