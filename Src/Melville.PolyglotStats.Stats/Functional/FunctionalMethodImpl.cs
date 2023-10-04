using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System;
using System.Collections;

namespace Melville.PolyglotStats.Stats.Functional;

public static class FunctionalMethodImpl
{
    public static TDest Map<TSource, TDest>(TSource item,
        params (TSource Source, TDest Dest)[] tuples)
    {
        return tuples.First(i => Object.Equals(item, i.Source)).Dest;
    }

    public static TDest Map<TSource, TDest>(TSource item, TDest defaultValue,
        params (TSource Source, TDest Dest)[] tuples)
    {
        return tuples.Where(i => Object.Equals(item, i.Source))
            .DefaultIfEmpty((Source: default!, Dest: defaultValue!))
            .First()
            .Dest;
    }

    public static IList<T> AsList<T>(this IEnumerable<T> item) =>
        item is IList<T> list ? list : item.ToList();

    public static IEnumerable<T> Repeat<T>(this IEnumerable<T> src, int copies)
    {
        if (copies < 1) return new T[0];
        var list = src.AsList();
        return list.Cycle(copies * list.Count);
    }

    public static IEnumerable<object> ObjectListWithNulls(this IEnumerable src)
    {
        foreach (var item in src)
        {
            yield return item;
        }
    }

    /// <summary>
    /// Convert a jagged array into a 2 dimensional array
    /// </summary>
    /// <typeparam name="T">Type of the elements</typeparam>
    /// <param name="data">2 dimensional array of arrays</param>
    /// <returns>2d array with the same data</returns>
    public static T[,] To2x2<T>(this IList<IList<T>> data)
    {
        var rows = data.Count;
        var cols = data.Max(i => i.Count);
        var ret = new T[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < data[i].Count; j++)
            {
                ret[i, j] = data[i][j];
            }
        }
        return ret;
    }
}