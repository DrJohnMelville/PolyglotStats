using Melville.PolyglotStats.Stats.Functional;

namespace Melville.PolyglotStats.Test.Stats.FunctionalMethodsTests;

public class FunctionalMethodImplTest
{
    [Fact]
    public void SimpleMap()
    {
        Assert.Equal("One", FunctionalMethodImpl.Map(1, (1,"One"), (2, "Two")) );
        Assert.Equal("One", FunctionalMethodImpl.Map(1, "Foo", (1,"One"), (2, "Two")) );
    }
    [Fact]
    public void SimpleMapDefault()
    {
        Assert.Equal("Foo", FunctionalMethodImpl.Map(4, "Foo", (1,"One"), (2, "Two")) );
    }
    [Fact]
    public void MapNull()
    {
        Assert.Equal("Three", FunctionalMethodImpl.Map<int?,string>(null, (1 as int?,"One"), (2, "Two"), (null, "Three")) );
        Assert.Equal("Three", FunctionalMethodImpl.Map<int?,string>(null, "Foo", (1 as int?,"One"), (2, "Two"), (null, "Three")) );
    }
}