using System;
using System.Collections.Generic;
using Melville.INPC;

namespace Melville.PolyglotStats.TableSource.Parser;

public partial class ParsedTable
{
    [FromConstructor] public ReadOnlyMemory<char> Name { get; }
    public ReadOnlyMemory<char>[] Titles { get; private set; } = Array.Empty<ReadOnlyMemory<char>>();

    public List<ReadOnlyMemory<char>[]> Rows { get; } = new();

    public void AddTitles(List<ReadOnlyMemory<char>> currentLine) => Titles = currentLine.ToArray();

    public void AddData(List<ReadOnlyMemory<char>> currentLine)
    {
        Rows.Add(currentLine.ToArray());
    }
}