using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions;

public static class RandomExtensions
{
    public static double NextDouble(this Random Generator, double MinValue, double MaxValue)
    {
        return Generator.NextDouble() * (MaxValue - MinValue) + MinValue;
    }

    public static double NextDouble(this Random Generator, double MaxValue)
    {
        // Checks //
        if (MaxValue is 1)
            return Generator.NextDouble();
        if (MaxValue < 1 && MaxValue > 0)
            return Generator.NextDouble();
        if (MaxValue < 0)
            return NextDouble(Generator, MaxValue, 0);
        // From 0 to Max //
        return NextDouble(Generator, 0, MaxValue);
    }
}
