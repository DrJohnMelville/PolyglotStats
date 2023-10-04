using System.Collections.Generic;
using Melville.PolyglotStats.Stats.Functional;

namespace Melville.PolyglotStats.Stats.Tables;

public class TableAxis<TStorage>: ITableAxis<TStorage>
{
    public string Name { get; }
    public IList<ITableRowOrColumn<TStorage>> Elements { get; }

    public TableAxis(string name, IEnumerable<ITableRowOrColumn<TStorage>> elements)
    {
        Name = name;
        Elements = elements.AsList();
    }
}