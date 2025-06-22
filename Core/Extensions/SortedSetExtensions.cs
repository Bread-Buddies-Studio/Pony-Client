using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions;

public static class SortedSetExtensions
{
    public static SortedSet<T> UpdateElement<T>(this SortedSet<T> set, T element)
    {
        // Update //
        set.Remove(element);
        set.Add(element);
        // Return new Set //
        return set;
    }

    /// <summary>
    /// Refreshes the element sorting in this <see cref="SortedSet{T}"/>/>
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// Class of the elements in this <see cref="SortedSet{T}"/>
    /// </typeparam>
    /// <remarks>
    /// Slow process, attempts to speed this method up would be greatly appreciated
    /// </remarks>
    public static void Update<T>(this SortedSet<T> set)
    {
        T[] cache = [.. set];
        set.Clear();

        foreach (var element in cache)
            set.Add(element);
    }

    /// <summary>
    /// Updates an individual element of type <typeparamref name="T"/> in this <see cref="SortedSet{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="set"></param>
    /// <param name="predicate"></param>
    /// <remarks>Warning: Experimental</remarks>
    [Experimental("UpdateSortedSetWithPredicate")]
    public static void Update<T>(this SortedSet<T> set, Func<T, bool> predicate)
    {
        Span<T> cache = [.. set.Where(predicate)];

        foreach (var element in cache)
        {
            set.Remove(element);
            set.Add(element);
        }

        Debug.WriteLine(cache.Length);
    }
}