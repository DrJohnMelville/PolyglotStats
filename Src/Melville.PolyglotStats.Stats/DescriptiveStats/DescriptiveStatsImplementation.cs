﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Accord.Statistics.Models.Regression.Linear;
using Melville.PolyglotStats.Stats.Functional;

namespace Melville.PolyglotStats.Stats.DescriptiveStats;

public static class DescriptiveStatsImplementation
{
    /// <summary>
    /// A "percentile" the element that is the given fraction through the sorted list
    /// </summary>
    /// <typeparam name="T">A comparable tyle</typeparam>
    /// <param name="items">the daat.</param>
    /// <param name="fraction">A number between zero and 1, inclusive, describing the desired fraction of the data</param>
    /// <returns>the first element smaller than the requested fraction</returns>
    public static T Centile<T>(this IEnumerable<T> items, double fraction)
        where T : IComparable<T> => Centile(items, i => i, fraction);

    public static T Median<T>(this IEnumerable<T> items) where T : IComparable<T> => Median(items, i => i);

    public static T Median<T, TRes>(this IEnumerable<T> items, Func<T, IComparable<TRes>> func) =>
        Centile(items, func, 0.5);

    public static T Centile<T, TRes>(this IEnumerable<T> items, Func<T, IComparable<TRes>> func, double fraction)
    {
        var ordered = items.OrderBy(func);
        var arr = ordered.ToList();
        return OrderedCentile(arr, fraction);
    }

    public static (double Mean, double StdDev) MeanAndStandardDeviation<T>(
        this IEnumerable<T> items, bool population = false) where T : IConvertible
    {
        {
            // ref: http://warrenseen.com/blog/2006/03/13/how-to-calculate-standard-deviation/
            double mean = 0.0;
            double sum = 0.0;
            double stdDev = 0.0;
            int n = 0;
            foreach (T genericVal in items)
            {
                var val = genericVal.ToDouble(CultureInfo.CurrentCulture);
                n++;
                double delta = val - mean;
                mean += delta / n;
                sum += delta * (val - mean);
            }

            if (1 < n)
                stdDev = Math.Sqrt(sum / (n - (population ? 0 : 1)));

            return (mean, stdDev);
        }
    }

    public static double StandardDeviation<T>(this IEnumerable<T> items, bool population = false) 
        where T: IConvertible => MeanAndStandardDeviation(items, population).StdDev;


    public static T OrderedCentile<T>(this IList<T> arr, double fraction)
    {
        if (arr.Count < 1) return default!;
        return arr[int.Clamp((int)(fraction * arr.Count), 0, arr.Count - 1)];
    }

    public static PolynomialRegression Fit<T>(this IEnumerable<T> items, Func<T, double> xFunc, Func<T, double> yFunc,
    int order = 1)
    {
      var itemList = items.AsList();
#pragma warning disable 0618
      // this is obsolete due to a new "learn" interface for many machine learning algorthms
      // unfortunately, on 5/14/17 the new interface is broken.
      // https://github.com/accord-net/framework/issues/434
      var regression = new PolynomialRegression(order);
      regression.Regress(itemList.Select(xFunc).ToArray(), itemList.Select(yFunc).ToArray());
#pragma warning restore 0618
      return regression;
    }

    public static (int numerator, int denominator) Fraction<T>(this IEnumerable<T> data, Func<T, bool> func)
    {
        int numerator = 0;
        int denominator = 0;
        foreach (var datum in data)
        {
            denominator++;
            if (func(datum))
            {
                numerator++;
            }
        }

        return (numerator, denominator);
    }

    public static string CountAndPercent<T>(this IEnumerable<T> data, Func<T, bool> func)
    {
        int numerator = 0;
        int denominator = 0;
        (numerator, denominator) = Fraction(data, func);
        return string.Format(denominator >= 100 ? "{0} ({1:##0.0}%)" : "{0} ({1:##0}%)", numerator,
            100.0 * numerator / denominator);
    }
}