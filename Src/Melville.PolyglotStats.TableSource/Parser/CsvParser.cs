using System;
using System.Collections.Generic;
using Melville.INPC;

namespace Melville.PolyglotStats.TableSource.Parser;

public readonly partial struct CsvParser
{
    [FromConstructor] private readonly ParsedTable target;
    private readonly List<ReadOnlyMemory<char>> currentLine = new();

    public void Parse(ReadOnlyMemory<char> memory)
    {
        if (!ReadOneLine(ref memory)) return;
        target.AddTitles(currentLine);
        while (true)
        {
            currentLine.Clear();
            if (!ReadOneLine(ref memory)) return;
            target.AddData(currentLine);
        }
    }

    private bool ReadOneLine(ref ReadOnlyMemory<char> memory)
    {
        var foundSomething = false;
        while (memory.Length > 0)
        {
            if (CheckForEndOfLine(ref memory)) return foundSomething;
            currentLine.Add(GetItem(ref memory));
            foundSomething = true;
            TrySkipComma(ref memory);
        }

        return foundSomething;
    }

    private static void TrySkipComma(ref ReadOnlyMemory<char> memory)
    {
        if (memory.Length > 0 && memory.Span[0] is ',')
            memory = memory[1..];
    }


    private bool CheckForEndOfLine(ref ReadOnlyMemory<char> memory)
    {
        var eolnFound = false;
        var span = memory.Span;
        for (int i = 0; i < span.Length; i++)
        {
            switch (span[i])
            {
                case '\r' or '\n':
                    eolnFound = true;
                    break;
                case var x when Char.IsWhiteSpace(x):
                    break;
                default:
                    TakePrefixAndIncrement(ref memory, i);
                    return eolnFound;
            }
        }

        memory = ReadOnlyMemory<char>.Empty;
        return eolnFound;
    }

    private enum CsvParseState {PreQuotedString, InQuotedString, AfterQuotedString}
    private ReadOnlyMemory<char> GetItem(ref ReadOnlyMemory<char> memory)
    {
        var span = memory.Span;
        var state = CsvParseState.PreQuotedString;
        for (int i = 0; i < span.Length; i++)
        {
            switch (span[i], state)
            {
                case (',' or '\r' or '\n', CsvParseState.PreQuotedString):
                    return TakePrefixAndIncrement(ref memory, i).TrimEnd();
                case (',' or '\r' or '\n', CsvParseState.AfterQuotedString):
                    return RemoveDoubleQuotes(TakePrefixAndIncrement(ref memory, i).TrimEnd()[1..^1]);
                case ('"', CsvParseState.PreQuotedString or CsvParseState.AfterQuotedString):
                    state = CsvParseState.InQuotedString;
                    break;
                case ('"', CsvParseState.InQuotedString):
                    state = CsvParseState.AfterQuotedString;
                    break;
            }
        }

        return TakePrefixAndIncrement(ref memory, memory.Length);
    }

    private ReadOnlyMemory<char> RemoveDoubleQuotes(ReadOnlyMemory<char> result) => 
        (result.Span.IndexOf('"') >= 0) ? result.ToString().Replace("\"\"", "\"").AsMemory() : result;

    private ReadOnlyMemory<char> TakePrefixAndIncrement(ref ReadOnlyMemory<char> memory, int prefixLength)
    {
        var ret = memory[..prefixLength];
        memory = memory[prefixLength..];
        return ret;
    }

}