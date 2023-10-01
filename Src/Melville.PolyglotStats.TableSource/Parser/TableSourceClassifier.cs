using System;
using System.Text.RegularExpressions;

namespace Melville.PolyglotStats.TableSource.Parser;

public static partial class TableSourceClassifier
{
    [GeneratedRegex("""
                    \A.*$\s?  # match anything for the first line
                    \|?(?:[-\:]+\|)+-*\s*$ # second line must be a sequence of 
                    """, RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline)]
    private static partial Regex MarkdownDetector();

    public static bool IsMarkdown(ReadOnlySpan<char> source) => MarkdownDetector().IsMatch(source);

    [GeneratedRegex(@"\A.*(?:,.*)+$", RegexOptions.Multiline)]
    private static partial Regex CsvDetector();

    public static bool IsCsv(ReadOnlySpan<char> item) => CsvDetector().IsMatch(item);
}