using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Melville.PolyglotStats.TableSource.Parser;

public static class EnumerateSegmentsImpl
{
    public static IEnumerable<ReadOnlyMemory<char>> SplitSource(
        this Regex delimiter, ReadOnlyMemory<char> source)
    {
        int beginning = 0;
        while (FindFirstInstance(delimiter, source[beginning..], out int start, out int end))
        {
            Debug.Assert(end > start);
            yield return source.Slice(beginning, start);
            beginning += end;
        }

        yield return source[beginning..];
    }

    public static bool FindFirstInstance(
        this Regex delimiter, ReadOnlyMemory<char> source, out int start, out int end)
    {
        var enumerator = delimiter.EnumerateMatches(source.Span);
        if (enumerator.MoveNext())
        {
            start = enumerator.Current.Index;
            end = start + enumerator.Current.Length;
            return true;
        }

        start = end = source.Length;
        return false;
    }
}