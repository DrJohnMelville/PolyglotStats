using System;
using System.Collections.Generic;
using Melville.INPC;

namespace Melville.PolyglotStats.TableSource.Parser;

public readonly partial struct SingleColumnParser
{
    [FromConstructor] private readonly ParsedTable target;
    private readonly List<ReadOnlyMemory<char>> scratch = new ();

    public void Parse(ReadOnlyMemory<char> memory)
    {
        using var enumerator = LineFinder.LineDelimiter().SplitSource(memory).GetEnumerator();
        if (!enumerator.MoveNext()) return;
        scratch.Add(enumerator.Current);
        target.AddTitles(scratch);

        while (enumerator.MoveNext())
        {
            scratch[0] = enumerator.Current;
            target.AddData(scratch);
        }
    }
}