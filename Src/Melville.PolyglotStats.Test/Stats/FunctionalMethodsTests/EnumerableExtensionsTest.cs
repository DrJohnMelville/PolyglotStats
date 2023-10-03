using Melville.PolyglotStats.Stats.Functional;
using EnumerableExtensions = Melville.Linq.EnumerableExtensions;

namespace Melville.PolyglotStats.Test.Stats.FunctionalMethodsTests;

public class EnumerableExtensionsTest
{

    [Fact]
    public void Cycle()
    {
        IEnumerable<int> tempQualifier = Enumerable.Range(1,3).Cycle(9);
        Assert.Equal("123123123", string.Join(string.Empty, tempQualifier.ToArray()));
    }

}