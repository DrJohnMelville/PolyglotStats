using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Melville.INPC;

namespace Melville.PolyglotStats.TableSource.Parser;

public static partial class LineFinder
{
    [GeneratedRegex(@"\s*[\r\n]+\s*")]
    public static partial Regex LineDelimiter();

}

public readonly partial struct MarkdownParser
{
    [FromConstructor] private readonly ParsedTable target;
    private readonly List<ReadOnlyMemory<char>> currentLine = new();


    [GeneratedRegex(@"\s*\|\s*")]
    private static partial Regex CellDelimiter();

    public void Parse(ReadOnlyMemory<char> sources)
    {
        var lines = LineFinder.LineDelimiter().SplitSource(sources).GetEnumerator();
        
        // titles
        if (!lines.MoveNext()) return;
        GetLine(lines.Current);
        target.AddTitles(currentLine);

        // fence
        if (!lines.MoveNext()) return;

        // values
        while (true)
        {
            currentLine.Clear();
            if (!lines.MoveNext()) break;
            GetLine(lines.Current.TrimEnd());
            target.AddData(currentLine);
        }
    }

    private void GetLine(ReadOnlyMemory<char> line)
    {
        foreach (var cell in CellDelimiter().SplitSource(line))
        {
            currentLine.Add(cell);
        }
    }
}