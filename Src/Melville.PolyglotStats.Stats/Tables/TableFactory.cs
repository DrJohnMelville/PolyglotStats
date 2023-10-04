using System;
using System.Collections.Generic;
using System.Linq;

namespace Melville.PolyglotStats.Stats.Tables;

public class RequireStruct<T> where T : struct { private RequireStruct() { } }
public class RequireClass<T> where T : class { private RequireClass() { } }

public static class TableFactory
{

    public static ITable<T> Table<T>(this IEnumerable<T> items, RequireStruct<T> reservedMustBeNull = null!) where  T:struct =>
        new ValueTable<T>(items);
    public static ITable<T> Table<T>(this IEnumerable<T> items, RequireClass<T> reservedMustBeNull = null!) where
        T: class=> new ReferenceTable<T>(items);
    public static ITable<T> Table<T>(this ISet<T> items, RequireClass<T> reservedMustBeNull = null!) where
        T: class=> new ReferenceTable<T>(items);

    public static ITable<T> WithCellFunction<T>(this ITable<T> table, Func<IEnumerable<T>, object> func,
        SummaryFunctionSelection selection = SummaryFunctionSelection.All)
    {
        return table.WithCellFunction((row, col, cell) => func(cell), selection);
    }

    public static ITable<T> WithCellFunction<T>(this ITable<T> table, Func<ITable<T>,ConfiguredCellFunction<T>> config)
    {
        var func = config(table);
        return table.WithCellFunction(func.Func, func.DefaultSelection);
    }
    ///selection parameter overrides the default
    public static ITable<T> WithCellFunction<T>(this ITable<T> table, Func<ITable<T>, ConfiguredCellFunction<T>> config,
        SummaryFunctionSelection selection)
    {
        var func = config(table);
        return table.WithCellFunction(func.Func, selection);
    }
    #region TableDefinitions

    private sealed class ReferenceTable<TItem> : TableImplementation<TItem, TItem> where TItem: class
    {
        public ReferenceTable(IEnumerable<TItem> items) : this(new ObjectIdentitySet<TItem>(items))
        {
        }

        public ReferenceTable(ObjectIdentitySet<TItem> items) : base(items)
        {
        }

        protected override TItem FromStorage(TItem item) => item;

    }


    public sealed class ValueBox<T> where T :struct
    {
        public T Value { get; }

        public ValueBox(T value)
        {
            Value = value;
        }
    }

    private sealed class ValueTable<T> : TableImplementation<T, ValueBox<T>> where T:struct
    {
        public ValueTable(IEnumerable<T> items) : 
            base(new ObjectIdentitySet<ValueBox<T>>(items.Select(i => new ValueBox<T>(i))))
        {
        }

        protected override T FromStorage(ValueBox<T> item) => item.Value;
    }


    #endregion
}