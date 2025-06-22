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
}
