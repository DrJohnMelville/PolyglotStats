using System.Collections.Generic;

namespace Melville.PolyglotStats.Stats.Tables;

public interface ITableAxis<TStorage>
{
    string Name { get; }
    IList<ITableRowOrColumn<TStorage>> Elements { get; }
}