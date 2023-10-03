using Melville.PolyglotStats.Stats.Functional;

namespace Melville.PolyglotStats.Test.Stats.FunctionalMethodsTests;

public sealed class ExpressionPrinterTest
{
    [Fact]
    public void SimplePropertyaCCESS()
    {
        Assert.Equal("Length", ExpressionPrinter.Print((string s)=>s.Length));
    }
    [Fact]
    public void NullCoalescing()
    {
        Assert.Equal("Reverse()", ExpressionPrinter.Print((string s)=>s.Reverse() ?? ""));
    }
}