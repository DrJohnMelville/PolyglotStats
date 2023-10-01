using FluentAssertions;
using Melville.PolyglotStats.TableSource.Parser;

namespace Melville.PolyglotStats.Test.TableSource.Parser;

public class CsvParserTest
{
    private readonly ParsedTable table = new ParsedTable("xxyy".AsMemory());

    [Theory]
    [InlineData("a,b,c", "a", "b", "c")]
    [InlineData("alpha, bravo  , charlie", "alpha", "bravo", "charlie")]
    [InlineData("alpha, bravo  , charlie\r\na,b,x", "alpha", "bravo", "charlie")]
    [InlineData("alpha, \"bravo\"  , charlie", "alpha", "bravo", "charlie")]
    [InlineData("alpha, \"bra,vo\"  , charlie", "alpha", "bra,vo", "charlie")]
    [InlineData("alpha, \"bra\r\r\nvo\"  , charlie", "alpha", "bra\r\r\nvo", "charlie")]
    [InlineData("alpha, \"bra\"\"vo\"  , charlie", "alpha", "bra\"vo", "charlie")]
    public void ParseSingleLine(string source, string a, string b, string c)
    {
        new CsvParser(table).Parse(source.AsMemory());
        table.Titles.Select(i=>i.ToString()).Should().BeEquivalentTo(
            new[]{a,b,c});
    }
}