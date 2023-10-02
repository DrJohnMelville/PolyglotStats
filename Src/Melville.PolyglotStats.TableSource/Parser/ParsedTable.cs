using System;
using System.Collections.Generic;
using System.Linq;
using Melville.INPC;
using Melville.PolyglotStats.TableSource.ModelGenerators;
using Melville.PolyglotStats.TableSource.TypeInference;

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

    public FieldRequest[] FieldRequests()=>
        Titles.Select(SingleFieldRequest).ToArray();

    private FieldRequest SingleFieldRequest(ReadOnlyMemory<char> name, int column)
    {
        return new FieldRequest(name.CanonicalName(), 
            InferType.Of(Rows.Select(i => i.ValueOrEmpty(column))));
    }
}