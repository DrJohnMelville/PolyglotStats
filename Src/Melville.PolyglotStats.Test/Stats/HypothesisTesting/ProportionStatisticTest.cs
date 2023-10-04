using Melville.PolyglotStats.Stats.HypothesisTesting;

namespace Melville.PolyglotStats.Test.Stats.HypothesisTesting;

public class ProportionStatisticTest
{
    [Fact]
    public void DifferenceOfProportionsTest()
    {
        var stat = ProportionStatistics.DifferenceOfProportions(38, 100, 102, 200);
        Assert.Equal(-2.13, stat.ZScore, 2);
      
    }
}