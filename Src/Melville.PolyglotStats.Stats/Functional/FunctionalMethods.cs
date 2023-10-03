using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System;

namespace Melville.PolyglotStats.Stats.Functional;

public static class FunctionalMethods
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
        .DefaultIfEmpty((Source:default!, Dest:defaultValue!))
        .First()
        .Dest;
    }

    public static IList<T> AsList<T>(this IEnumerable<T> source) =>
      source as IList<T> ?? source.ToList();
  }