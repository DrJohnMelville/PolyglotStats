using Melville.FileSystem;
using Melville.PolyglotStats.TableSource.ModelGenerators;
using Melville.PolyglotStats.TableSource.Parser;

namespace Melville.PolyglotStats.Test.TableSource;

public class IntegrationTest
{
    [Theory]
    [InlineData("""
                ##### Table 1
                Column a, column b
                1, true
                , f
                12, yes
                ##### Tab_b
                Names
                John
                Jacob
                """,
        """
        public class DataClass {
            public record Table1Class (
                System.Int32? ColumnA,
                bool ColumnB
            );
            public readonly Table1Class[] Table1 = new Table1Class[] {
                new (1, true),
                new (default, false),
                new (12, true),
            };
            public record TabBClass (
                string Names
            );
            public readonly TabBClass[] TabB = new TabBClass[] {
                new (@"John"),
                new (@"Jacob"),
            };
        }
        public readonly DataClass Data = new();
        """)]
    public async Task GenerateDataModed(string source, string destination)
    {
        var mock = new Mock<IDiskFileSystemConnector>();
        mock.Setup(i => i.FileFromPath(It.IsAny<string>())).Returns(Mock.Of<IFile>());
        var output = await GeneratorFacade.QueryToCode(source, mock.Object);
        output.Should().Be(destination);
    }
}