using System.Collections.Generic;
using System;
using System.Linq;

namespace Melville.PolyglotStats.Stats.Tables;

public static class CellFunctions
  {
    public static string PercentAndFraction<T>(this IEnumerable<T>items, Func<T, bool> predicate)
    {
      int numerator = 0;
      int denominator = 0;
      foreach (var item in items)
      {
        denominator++;
        if (predicate(item))
        {
          numerator++;
        }
      }
      return PercentAndFraction(numerator, denominator);
    }
    public static string PercentAndFraction<TNum, TDenom>(IEnumerable<TNum> numerator, IEnumerable<TDenom> denominator) =>
      PercentAndFraction(numerator.Count(), denominator.Count());
    public static string PercentAndFraction(double numerator, double denominator) => 
      denominator == 0.0?$"{numerator}/{denominator}":string.Format(PercentAndFractionFormat(denominator),
      100 * numerator / denominator, numerator, denominator);
    private static string PercentAndFractionFormat(double denominator) => 
      denominator >= 100 ? "{0:#0.0}% ({1}/{2})" : "{0:#0}% ({1}/{2})";

    private static string AxisPercentFormat(int denominator) => denominator >= 100 ? "{0} ({1:##0.0%})" : "{0} ({1:##0%)}";
    private static string AxisPercentage<T>(IEnumerable<T> axis, IEnumerable<T> cell)
    {
      var numerator = cell?.Count() ??0;
      var denominator = axis?.Count() ?? 1;
      return string.Format(AxisPercentFormat(denominator), numerator, (1.0 * numerator) / denominator);
   }
    //unused is not used, but it's presence allows the type resolution to work automatically
    public static ConfiguredCellFunction<T> RowPercentage<T>(ITable<T> table) =>
      new ConfiguredCellFunction<T>((row,col,cell)=>AxisPercentage(row,cell), 
        SummaryFunctionSelection.Cell | SummaryFunctionSelection.Column);
    //unused is not used, but it's presence allows the type resolution to work automatically
    public static ConfiguredCellFunction<T> ColumnPercentage<T>(ITable<T> table) =>
      new ConfiguredCellFunction<T>((row,col,cell)=>AxisPercentage(col,cell), 
        SummaryFunctionSelection.Cell | SummaryFunctionSelection.Row);

    public static ConfiguredCellFunction<T> TablePercentage<T>(ITable<T> table) =>
      new ConfiguredCellFunction<T>((row,col,cell)=>AxisPercentage(table.AllValues(),cell),
        SummaryFunctionSelection.Cell | SummaryFunctionSelection.Column | SummaryFunctionSelection.Row);
  }

  public class ConfiguredCellFunction<T>
  {
    public Func<IEnumerable<T>, IEnumerable<T>, IEnumerable<T>, string> Func { get; }
    public SummaryFunctionSelection DefaultSelection { get; }

    public ConfiguredCellFunction(Func<IEnumerable<T>, IEnumerable<T>, IEnumerable<T>, string> func, SummaryFunctionSelection defaultSelection)
    {
      Func = func;
      DefaultSelection = defaultSelection;
    }
  }