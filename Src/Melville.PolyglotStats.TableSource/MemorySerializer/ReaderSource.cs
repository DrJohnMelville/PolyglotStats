using System.IO;

namespace Melville.PolyglotStats.TableSource.MemorySerializer;

public static class ReaderSource
{
    public static string Code() =>
        new StreamReader(
            typeof(ReaderSource).Assembly.GetManifestResourceStream(
            typeof(ReaderSource), "MemoryReader.cs")!).ReadToEnd();
}