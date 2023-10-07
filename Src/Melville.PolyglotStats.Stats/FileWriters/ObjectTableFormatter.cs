using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Melville.PolyglotStats.Stats.FileWriters;

public static partial class ObjectTableFormatter
{
    public static IEnumerable<IList> Dump<T>(IEnumerable<T> objects)
    {
        var fields = typeof(T).GetProperties().ToList();
        yield return fields.Select(i => UnPascalCase(i.Name)).ToList();
        foreach (var row in objects)
        {
            yield return fields.Select(i => ProcessField(i.GetMethod?.Invoke(row, null))??"").ToList();
        }
    }

    private static object? ProcessField(object? field) => field switch
        {
            int i => i,
            double d => d,
            DateTime d => d.ToShortDateString(),
            _ => ProcessString(field?.ToString())
        };

    private static object? ProcessString(string? field)
    {
        if (field == null) return null;
        if (int.TryParse(field, out var intValue)) return intValue;
        if (double.TryParse(field, out var doubleValue)) return doubleValue;
        return field;
    }

    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex PascalCaseFinder();

    private static string UnPascalCase(string input) =>
        PascalCaseFinder().Replace(input, "$1 $2");
}