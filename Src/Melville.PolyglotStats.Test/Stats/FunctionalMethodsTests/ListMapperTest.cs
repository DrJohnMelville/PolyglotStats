using Melville.PolyglotStats.Stats.Functional;

namespace Melville.PolyglotStats.Test.Stats.FunctionalMethodsTests;

public class ListMapperTest
{

    [Theory]
    [InlineData(1, "One")]
    [InlineData(2, "Two")]
    [InlineData(3, "Three")]
    [InlineData(0, "klhdk")]
    public void ListMappingTest(int result, string key)
    {
        var mapper = ListMapping.Create("One", "Two", "Three");
        mapper.Map(key).Should().Be(result);
    }
}