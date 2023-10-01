using Melville.PolyglotStats.TableSource.TypeInference;

namespace Melville.PolyglotStats.Test.TableSource;
public class TypeInferneceTest
{
    private InferredType InferFrom(params string[] items)
    {
        return InferType.Of(items.Select(i => i.AsMemory()));
    }

    [Fact]
    public void InferIntType()
    {
        var inferred = InferFrom("1", "2", "3", "4");
        inferred.Should().BeAssignableTo<InferredNumberType<int>>();
    }
    [Fact]
    public void InferDateType()
    {
        var inferred = InferFrom("1/2/33", "Dec 1, 2022");
        inferred.Should().BeAssignableTo<InferredDateType>();
    }
    [Fact]
    public void InferBooleanType()
    {
        var inferred = InferFrom("Yes", "y", "true", "T", "present", "positive", "1",
                                 "No", "n", "false", "f", "absent", "negative", "0");
        inferred.Should().BeAssignableTo<InferredBooleanType>();
    }
    [Fact]
    public void InferDoubleType()
    {
        var inferred = InferFrom("1", "2", "3.5", "4");
        inferred.Should().BeAssignableTo<InferredNumberType<double>>();
    }
    [Fact]
    public void InferNullableIntType()
    {
        var inferred = InferFrom("1", "2", "", "4");
        inferred.Should().BeAssignableTo<NullableWrapper>();
    }

    [Fact]
    public void InferStringType() =>
        InferFrom("Hello", "world").Should().BeAssignableTo<InferredStringType>();
}
