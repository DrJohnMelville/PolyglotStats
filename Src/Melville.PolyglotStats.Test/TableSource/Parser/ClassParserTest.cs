using System.Text;
using FluentAssertions;
using Melville.FileSystem;
using Melville.PolyglotStats.TableSource.Parser;
using Moq;

namespace Melville.PolyglotStats.Test.TableSource.Parser;

public class ClassParserTest
{
    private readonly Mock<IDiskFileSystemConnector> fileSystemMock = new(MockBehavior.Strict);

    [Theory]
    [InlineData("TwoCharts.xls")]
    [InlineData("TwoCharts.xlsx")]
    public async Task ParseExcelFiles(string fileName)
    {
        fileSystemMock.Setup(i => i.FileFromPath(It.IsAny<string>())).Returns(
            LoadFileFromResource);
        var result = await DoParse(fileName);
        result.Tables.Should().HaveCount(2);
        TableAssertions(result.Tables[0]);
        TableAssertions(result.Tables[1]);
    }

    private IFile LoadFileFromResource(string name)
    {
        return new MemoryFile("h:\\www\\"+name,
            GetType().Assembly.GetManifestResourceStream(GetType(), name)??
            throw new FileNotFoundException("No file"));
    }

    [Fact]
    public async Task ParseDiskFiles()
    {
        fileSystemMock.Setup(i=>i.FileFromPath("SingleTable.csv")).Returns(
            CreateFile(@"e:\ddd\SingleTable.csv", """
                                                  Col a, col_b, 3rdCol
                                                  1,   2, three
                                                  4,  , "five,"
                                                  5, 7, 8
                                                  """u8));
        fileSystemMock.Setup(i=>i.FileFromPath("SingleTable.md")).Returns(
            CreateFile(@"e:\ddd\SingleTable.md", """
                                                  Col a| col_b  | 3rdCol
                                                  ---|---|---
                                                  1 |    2| three
                                                  4 |  | five,
                                                  5| 7|8 
                                                  """u8));

        var result = await DoParse("""
                                   SingleTable.csv
                                   SingleTable.md
                                   """);
        result.Tables.Should().HaveCount(2);
        TableAssertions(result.Tables[0]);
        TableAssertions(result.Tables[1]);
    }

    private static MemoryFile CreateFile(string fileName, ReadOnlySpan<byte> content) => new(fileName, content.ToArray());

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
        TableAssertions(result.Tables[0]);
    }

    private static void TableAssertions(ParsedTable table)
    {
        table.Name.ToString().Should().StartWith("SingleTable");
        table.Titles.Should().HaveCount(3);
        table.Titles.Select(i => i.ToString()).Should().BeEquivalentTo("Col a", "col_b", "3rdCol");
        table.Rows.Should().HaveCount(3);
        table.Rows[0].Select(i => i.ToString()).Should()
            .BeEquivalentTo("1", "2", "three");
        table.Rows[1].Select(i => i.ToString()).Should()
            .BeEquivalentTo("4", "", "five,");
        table.Rows[2].Select(i => i.ToString()).Should()
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
        result.Tables[1].Rows[0].Select(i => i.ToString())
            .Should().BeEquivalentTo("7", "8", "9")
            ;
    }
}