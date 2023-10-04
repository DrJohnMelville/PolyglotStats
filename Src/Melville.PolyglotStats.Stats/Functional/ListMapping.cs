using System.Collections;
using System.Collections.Generic;
using Melville.INPC;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace Melville.PolyglotStats.Stats.Functional;

public static class ListMapping
{
    public static ListMapping<T> Create<T>(params T[] items) => new((IList<T>)items);
}
public readonly partial struct ListMapping<T>
{
    [FromConstructor] private readonly IList<T> items;
        
    public int Map(T key)
    {
        return items.IndexOf(key) + 1;
    }
}