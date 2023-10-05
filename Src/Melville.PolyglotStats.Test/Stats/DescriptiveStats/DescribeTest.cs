using Melville.INPC;
using Melville.PolyglotStats.Stats.DescriptiveStats;
using Melville.TestHelpers.StringDatabase;

namespace Melville.PolyglotStats.Test.Stats.DescriptiveStats;

public partial class DescribeTest: IClassFixture<StringTestDatabase>
{
    [FromConstructor] private StringTestDatabase database;

    [Fact]
    public void DescribeInts()
    {
        database.AssertDatabase(
            new[] { 1, 2, 2, 3, 4, 5 }.Describe().RenderAsHtml());
    }

    [Fact]
    public void StringData()
    {
        database.AssertDatabase(new [] {"Apple","Bat","Chichannuah"}.Describe().RenderAsHtml());
    }
}