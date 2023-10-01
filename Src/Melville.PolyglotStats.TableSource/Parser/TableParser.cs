using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using FSharp.Compiler.Syntax;
using FSharp.Compiler.Text;
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
        return await ParseFromSpan(source, "SingleTable".AsMemory());
    }

    private async Task<ParsedTableSet> ParseFromSpan(ReadOnlyMemory<char> innerSource, ReadOnlyMemory<char> name)
    {
        if (!TableDelimiter().FindFirstInstance(innerSource, out _, out _))
        {
            await ParseSingleTableAsync(name, innerSource);
        }
        else
        {
            foreach (var subTable in TableDelimiter().SplitSource(innerSource))
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
        else if (TableSourceClassifier.IsFile(memory, disk)) await ReadFileList(memory);
        if (table.Titles.Length > 0) output.Tables.Add(table);
    }

    private async Task ReadFileList(ReadOnlyMemory<char> source)
    {
        foreach (var path in LineFinder.LineDelimiter().SplitSource(source))
        {
            var file = disk.FileFromPath(path.ToString());
            if (file.Exists()) await ReadSingleFile(file);
        }
    }

    private Task ReadSingleFile(IFile file)
    {
        return IsExcelFile(file.Path)?
            new ExcelFileReader(output).ParseAsync(file):
            ReadTextFile(file);
    }

    private bool IsExcelFile(ReadOnlySpan<char> path) =>
        path.EndsWith(".xlsx", StringComparison.InvariantCultureIgnoreCase) ||
        path.EndsWith(".xlsb", StringComparison.InvariantCultureIgnoreCase) ||
        path.EndsWith(".xls", StringComparison.InvariantCultureIgnoreCase);


    private async Task ReadTextFile(IFile file)
    {
        await using var stream = await file.OpenRead();
        var text = await new StreamReader(stream).ReadToEndAsync();
        await ParseFromSpan(text.AsMemory(),
            Path.GetFileNameWithoutExtension(file.Path).ToCharArray());
    }
}