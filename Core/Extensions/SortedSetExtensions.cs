using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions;

public static class SortedSetExtensions
{
    public static SortedSet<T> UpdateElement<T>(this SortedSet<T> set, T element)
    {
        // Update //
        if (set.Remove(element))
            set.Add(element);
        // Return new Set //
        return set;
    }
}
