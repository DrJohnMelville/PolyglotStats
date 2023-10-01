using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Melville.PolyglotStats.TableSource.ModelGenerators;

namespace Melville.PolyglotStats.Test.TableSource.ModelGenerators;
public class RenamerTest
{
    [Theory]
    [InlineData("Hello", "Hello")]
    [InlineData("1Hello", "_1Hello")]
    [InlineData("Hello world", "HelloWorld")]
    [InlineData("Hello_!@#$%^World", "HelloWorld")]
    public void RenameTest(string source, string destination) =>
        source.AsMemory().CanonicalName().ToString().
            Should().Be(destination);
}
