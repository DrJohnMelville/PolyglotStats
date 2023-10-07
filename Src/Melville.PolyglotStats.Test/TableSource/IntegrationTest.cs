using System.Text.RegularExpressions;
using Melville.FileSystem;
using Melville.PolyglotStats.TableSource.ModelGenerators;
using Melville.PolyglotStats.TableSource.Parser;

namespace Melville.PolyglotStats.Test.TableSource;

public partial class IntegrationTest
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
        #define InsideGeneratedCode
        using System;
        using System.IO;
        using System.IO.MemoryMappedFiles;
        using System.Runtime.InteropServices;
        using System.Text;
        
        public class DataClass {
        #if !InsideGeneratedCode
        using System;
        using System.IO;
        using System.IO.MemoryMappedFiles;
        using System.Runtime.InteropServices;
        using System.Text;
        
        namespace Melville.PolyglotStats.TableSource.MemorySerializer;
        #endif 
        
        public sealed class MemoryReader : IDisposable
        {
            private readonly MemoryMappedFile mmf;
            private readonly Stream stream;
        
            public  const long maxSize = 1024 * 1024 * 1024;
        
            public MemoryReader(string name)
            {
                mmf = MemoryMappedFile.OpenExisting(name);
                stream = mmf.CreateViewStream(0, maxSize);
            }
        
            public T Read<T>() where T : struct
            {
                T ret = default;
                var span = MemoryMarshal.Cast<T, byte>(MemoryMarshal.CreateSpan(ref ret, 1));
                stream.ReadAtLeast(span, span.Length);
                return ret;
            }
        
            public void Dispose()
            {
                stream.Dispose();
                mmf.Dispose();
            }
        
            public string ReadString()
            {
                var length = Read<int>();
                Span<byte> buffer = stackalloc byte[length];
                stream.ReadAtLeast(buffer, length);
                return Encoding.UTF8.GetString(buffer);
            }
        }
            public record Table1Class (
                System.Int32? ColumnA,
                bool ColumnB
            );
            public readonly Table1Class[] Table1 = ReadTable1Class();
            private static Table1Class[] ReadTable1Class()
            {
                using var reader = new MemoryReader("00000000-0000-0000-0000-000000000000");
                var ret = new Table1Class[3];
                for (int i = 0; i < ret.Length; i++)
                    ret[i] = new (reader.Read<byte>()==0?null:reader.Read<System.Int32>(), reader.Read<bool>());
                return ret;
                }
            public record TabBClass (
                string Names
            );
            public readonly TabBClass[] TabB = ReadTabBClass();
            private static TabBClass[] ReadTabBClass()
            {
                using var reader = new MemoryReader("00000000-0000-0000-0000-000000000000");
                var ret = new TabBClass[2];
                for (int i = 0; i < ret.Length; i++)
                    ret[i] = new (reader.ReadString());
                return ret;
                }
        }
        public static readonly DataClass Data = new();
        """)]
    public async Task GenerateDataModel(string source, string destination)
    {
        using var output = await GenerateFromSource(source);
        ReplaceGuid(output.Code).Should().Be(destination);
    }

    private static async Task<GeneratedCodeResult> GenerateFromSource(string source)
    {
        var mock = new Mock<IDiskFileSystemConnector>();
        mock.Setup(i => i.FileFromPath(It.IsAny<string>())).Returns(Mock.Of<IFile>());
        return await GeneratorFacade.QueryToCode(source, mock.Object);
    }
    
    [GeneratedRegex(@"[a-fA-F0-9]{8}-(?:[a-fA-F0-9]{4}-){3}[a-fA-F0-9]{12}")]
    private static partial Regex GuidFinder();

    public string ReplaceGuid(string s) =>
        GuidFinder().Replace(s, "00000000-0000-0000-0000-000000000000");

}