using System.Text.RegularExpressions;
using FluentAssertions;
using Melville.PolyglotStats.TableSource.Parser;

namespace Melville.PolyglotStats.Test.TableSource.Parser;

public class MarkdownParserTest
{
    private readonly ParsedTable table = new ParsedTable("xxyy".AsMemory());

    [Fact]
    public void EnumerateSegmentsTest()
    {
        var regex = new Regex("ab");
        regex.SplitSource("OneabtwoabThreeAb".AsMemory())
            .Select(i => i.ToString())
            .Should().BeEquivalentTo("One", "two", "ThreeAb");
    }
    [Theory]
    [InlineData("a|b|c\r\n---|---|---", "a", "b", "c")]
    public void ParseSingleLine(string source, string a, string b, string c)
    {
        new MarkdownParser(table).Parse(source.AsMemory());
        table.Titles.Select(i=>i.ToString()).Should().BeEquivalentTo(
            new[]{a,b,c});
    }
}