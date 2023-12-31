﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Melville.PolyglotStats.Stats.HypothesisTesting;

namespace Melville.PolyglotStats.Stats.Tables;

public interface ICanRenderASHtml
{
    string RenderAsHtml();
}
public interface ITable<TItem>: ICanRenderASHtml
{
    // Create Rows And Columns
    ITable<TItem> WithRows<TKey>(Expression<Func<TItem, TKey>> selector);
    ITable<TItem> WithRows<TKey>(string name, Func<TItem, TKey> selector);
    ITable<TItem> WithColumns<TKey>(Expression<Func<TItem, TKey>> selector);
    ITable<TItem> WithColumns<TKey>(string name, Func<TItem, TKey> selector);
    ITable<TItem> WithExplicitColumn(string name, params Expression<Func<TItem, bool>>[] selectors);
    ITable<TItem> WithExplicitRow(string name, params Expression<Func<TItem, bool>>[] selectors);

    ITable<TItem> WithCellFunction(Func<IEnumerable<TItem>, IEnumerable<TItem>, IEnumerable<TItem>, object> func,
        SummaryFunctionSelection selection = SummaryFunctionSelection.All);
    ITable<TItem> WithFormatter<T>(Func<T, string> func);

    IEnumerable<TItem> RowValues(int i);
    IEnumerable<TItem> ColumnValues(int p0);
    IEnumerable<TItem> CellValues(int row, int col);
    IEnumerable<TItem> AllValues();
    ChiSquaredStatisic ChiSquared();
}