using System;
using System.Text.RegularExpressions;

namespace Melville.PolyglotStats.TableSource.ModelGenerators;

public static partial class Renamer
{
    public static ReadOnlyMemory<char> CanonicalName(this ReadOnlyMemory<char> input) => input
        .TryReplace(SpaceWithSuffix(), m=>m.Groups[1].Value.ToUpper())
        .TryReplace(AnySpace(), "")
        .TryReplace(FirstNumber(), "_$0");

    [GeneratedRegex(@"\A[0-9]")]
    private static partial Regex FirstNumber();
    [GeneratedRegex(@"[^a-zA-Z0-9]+([a-z])")]
    private static partial Regex SpaceWithSuffix();
    [GeneratedRegex(@"[^a-zA-Z0-9]+")]
    private static partial Regex AnySpace();

    public static ReadOnlyMemory<char> TryReplace(
        this ReadOnlyMemory<char> input, Regex regex, string replacement) => 
        regex.IsMatch(input.Span) ? regex.Replace(input.ToString(), replacement ).AsMemory() : input;
    public static ReadOnlyMemory<char> TryReplace(
        this ReadOnlyMemory<char> input, Regex regex, MatchEvaluator replacement) => 
        regex.IsMatch(input.Span) ? regex.Replace(input.ToString(), replacement ).AsMemory() : input;
}