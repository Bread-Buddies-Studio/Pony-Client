using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions;

public static class RangeExtensions
{
    public static bool Contains(this Range range, int index)
    {
        // Conditions //
        if (index >= range.Start.Value && range.End.IsFromEnd)
            return true;
        // If in between //
        return index >= range.Start.Value && index <= range.End.Value;
    }
}
