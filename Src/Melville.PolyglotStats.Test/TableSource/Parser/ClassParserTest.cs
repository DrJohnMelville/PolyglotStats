using FluentAssertions;
using Melville.FileSystem;
using Melville.PolyglotStats.TableSource.Parser;
using Moq;

namespace Melville.PolyglotStats.Test.TableSource.Parser;

public class ClassParserTest
{
    private readonly Mock<IDiskFileSystemConnector> fileSystemMock = new(MockBehavior.Strict);
    
    [Theory]
    [InlineData("""
       Col a, col_b, 3rdCol
       1,   2, three
       4,  , "five,"
       5, 7, 8
       """)]
    [InlineData("""
       Col a| col_b  | 3rdCol
       ---|---|---
       1 |    2| three
       4 |  | five,
       5| 7|8 
       """)]
    public async Task Parse(string input)
    {
        var result = await DoParse(input);

        result.Tables.Should().HaveCount(1);
        result.Tables[0].Name.ToString().Should().Be("SingleTable");
        result.Tables[0].Titles.Should().HaveCount(3);
        result.Tables[0].Titles.Select(i=>i.ToString()).Should().BeEquivalentTo("Col a", "col_b", "3rdCol");
        result.Tables[0].Rows.Should().HaveCount(3);
        result.Tables[0].Rows[0].Select(i => i.ToString()).Should()
            .BeEquivalentTo("1", "2", "three");
        result.Tables[0].Rows[1].Select(i => i.ToString()).Should()
            .BeEquivalentTo("4", "", "five,");
        result.Tables[0].Rows[2].Select(i => i.ToString()).Should()
            .BeEquivalentTo("5", "7", "8");
    }

    private ValueTask<ParsedTableSet> DoParse(string input) => 
        new TableParser(input.AsMemory(), fileSystemMock.Object).ParseAsync();

    [Theory]
    [InlineData("""
                ##### Table1
                a,b, c
                1,2,4
                ##### Table 2
                d|e|   f
                ---|---|---
                7|8|9
                """)]
    public async Task Parse2Tables(string text)
    {
        var result = await DoParse(text);

        result.Tables.Should().HaveCount(2);
    }
}