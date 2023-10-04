using System.Collections.Generic;

namespace Melville.PolyglotStats.Stats.Tables;

public class TableRowOrColumn<T>: ITableRowOrColumn<T>
{
    public string Name { get; }
    public ObjectIdentitySet<T> Elements { get; }

    public TableRowOrColumn(string name, IEnumerable<T> elements)
    {
        Name = name;
        Elements = new ObjectIdentitySet<T>(elements);
    }
}