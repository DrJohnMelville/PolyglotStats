using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Melville.PolyglotStats.Stats.Functional;

public static class EnumerableExtensions
{
    /// <summary>
    /// Create an infinite enumaration that cycles through the given list over and over
    /// </summary>
    /// <typeparam name="T">Type of the items in the list</typeparam>
    /// <param name="source">List to cycle through</param>
    /// <returns>An infinite enumeration of the items in the surce list repeated over and over.</returns>
    public static IEnumerable<T> Cycle<T>(this IEnumerable<T> source)
    {
        var elts = source.ToList();
        if (elts.Count == 0) yield break;
        int current = 0;
        while (true)
        {
            yield return elts[current];
            current = (current + 1) % elts.Count;
        }
    }

    /// <summary>
    /// Returns an enumeration of a given length, repeating elements as needed.
    /// </summary>
    /// <typeparam name="T">Type of the enumerated elements</typeparam>
    /// <param name="source">Elements to enumerate</param>
    /// <param name="length">Length of the desired enumeration</param>
    /// <returns>enumeration of the given length</returns>
    public static IEnumerable<T> Cycle<T>(this IEnumerable<T> source, int length) =>
        source.Cycle().Take(length);
}