using Accord;
using Melville.PolyglotStats.Stats.FileWriters;
using Melville.PolyglotStats.Stats.Functional;
using Melville.TestHelpers.StringDatabase;

namespace Melville.PolyglotStats.Test.Stats.FileWritersTest;

public sealed class HtmlWriterTest: IClassFixture<StringTestDatabase>
{
    private readonly StringTestDatabase data;

    public HtmlWriterTest(StringTestDatabase data)
    {
        this.data = data;
//        data.Recording = true;
    }

    [Fact]
    public void WithDataRowTest()
    {
        data.AssertDatabase(new HtmlTable().WithDataRow<int>("Data",
                i=>i.Count.ToString(), Enumerable.Range(1,10).AsList(), Enumerable.Range(1,20).AsList())
            .ToHtmlText().ToString());
    }

    [Fact]
    public void SimpleTable()
    {
        var actual = new HtmlTable()
            .WithRow("A","B").WithRow("C","D").ToHtmlText().ToString();
        Assert.Equal("""
                       <table>
                         <tr>
                           <td>A</td>
                           <td>B</td>
                         </tr>
                         <tr>
                           <td>C</td>
                           <td>D</td>
                         </tr>
                       </table>
                       """, actual);
    }

    [Fact]
    public void SimpleNNonsquareTable()
    {
        var actual = new HtmlTable()
            .WithRow("A","B").WithRow("C").ToHtmlText().ToString();
        Assert.Equal("""
                       <table>
                         <tr>
                           <td>A</td>
                           <td>B</td>
                         </tr>
                         <tr>
                           <td>C</td>
                           <td colspan="1" />
                         </tr>
                       </table>
                       """, actual);
    }

    [Fact]
    public void BugCheck()
    {
        new HtmlTable()
            .WithRow(HtmlTable.TD("This is a summary row", ("colspan", 13)));
    }
}