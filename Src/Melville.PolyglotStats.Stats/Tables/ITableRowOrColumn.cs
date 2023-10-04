namespace Melville.PolyglotStats.Stats.Tables;

public interface ITableRowOrColumn<TStorage>
{
    string Name { get; }
    ObjectIdentitySet<TStorage> Elements { get; }
}