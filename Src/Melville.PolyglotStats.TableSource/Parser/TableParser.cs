using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FSharp.Compiler.Syntax;
using Melville.FileSystem;
using Melville.INPC;
using Microsoft.CodeAnalysis.VisualBasic;

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

public class ParsedTableSet
{
    public List<ParsedTable> Tables = new List<ParsedTable>();
}


public readonly partial struct TableParser
{
    [FromConstructor] private readonly ReadOnlyMemory<char> source;
    [FromConstructor] private readonly IDiskFileSystemConnector disk;
    private readonly ParsedTableSet output = new();

    [GeneratedRegex(@"^\#{5}\s*", RegexOptions.Multiline)]
    private static partial Regex TableDelimiter();



    public async ValueTask<ParsedTableSet> ParseAsync()
    {
        if (!TableDelimiter().FindFirstInstance(source, out _, out _))
        {
            await ParseSingleTableAsync("SingleTable".AsMemory(), source);

        }
        else
        {
            foreach (var subTable in TableDelimiter().SplitSource(source))
            {
                if (LineFinder.LineDelimiter().FindFirstInstance(subTable, out var end, out var next))
                {
                   await ParseSingleTableAsync(subTable[..end], subTable[next..]);
                }
            }
        }
        return output;
    }

    private async Task ParseSingleTableAsync(ReadOnlyMemory<char> tableName, ReadOnlyMemory<char> memory)
    {
        var table = new ParsedTable(tableName);
        if (TableSourceClassifier.IsCsv(memory.Span)) new CsvParser(table).Parse(memory);
        else if (TableSourceClassifier.IsMarkdown(memory.Span)) new MarkdownParser(table).Parse(memory);
        if (table.Titles.Length > 0) output.Tables.Add(table);
    }
}