using System;
using System.Collections.Generic;
using System.Linq;
using Melville.INPC;
using Melville.PolyglotStats.Stats.FileWriters;
using Melville.PolyglotStats.Stats.Functional;

namespace Melville.PolyglotStats.Stats.DescriptiveStats;

public static class VariableDescription
{
    public static HtmlTable Describe<TRow, TItem>(this IEnumerable<TRow> data, Func<TRow, TItem> selector) =>
        data.Select(selector).Describe();
    public static HtmlTable Describe<T>(this IEnumerable<T> data) =>
        Describe((IList<T>)data.AsList());

    public static HtmlTable Describe<T>(this IList<T> data)
    {
        return new VariableDescriber<T>(data).Describe();
    }
}

public readonly partial struct VariableDescriber<T>
{
    [FromConstructor] private readonly IList<T> data;
    private readonly HtmlTable target = new();

    public HtmlTable Describe()
    {
        DescribeList();
        if (data.Count < 1) return target;
        TryWriteSummaryStats(data);
        TopValues();
        return target;

    }

    private void TryWriteSummaryStats(IList<T> list)
    {
        if (typeof(T).IsAssignableTo(typeof(IConvertible)))
            MeanAndStdDev(list.OfType<IConvertible>());
        if (typeof(T).IsAssignableTo(typeof(IComparable)))
            Centiles(list.OfType<IComparable>());
    }

    private void DescribeList()
    {
        target.WithRow("Type", typeof(T).ToString());
        target.WithRow("Count", data.Count);
    }

    private void MeanAndStdDev(IEnumerable<IConvertible> convertibleList)
    {
        try
        {
            var (mean, stdDev) = convertibleList.MeanAndStandardDeviation();
            target
                .WithRow("Mean", mean)
                .WithRow("Standard Deviation", stdDev);
        }
        catch (FormatException )
        {
            // If I cannot convert to double then do not compute mean and std dev
        }
    }
    private void Centiles(IEnumerable<IComparable> items)
    {
        var ordered = items.OrderBy(i => i).ToList();
        target.WithRow("Centiles");
        target.WithRow("Max", ordered[^1]);
        target.WithRow("75%", ordered[(int)(0.75 * ordered.Count)]);
        target.WithRow("Median", ordered[(int)(0.5 * ordered.Count)]);
        target.WithRow("25%", ordered[(int)(0.25 * ordered.Count)]);
        target.WithRow("Min", ordered[0]);
    }
    private void TopValues()
    {
        var top = data.GroupBy(i => i)
            .OrderByDescending(i => i.Count())
            .Take(10);
        target.WithRow("10 Top Values");
        foreach (var row in top)
        {
            target.WithRow(PrintValue(row) , (row.Count() * 1.0 / data.Count).ToString("##0.0%"));
        }
    }

    private static string PrintValue(IGrouping<T, T>? row)
    {
        var ret = row?.Key?.ToString();
        return string.IsNullOrWhiteSpace(ret) ? "<null>" : ret;
    }
}