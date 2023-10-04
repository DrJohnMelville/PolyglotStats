using Melville.PolyglotStats.TableSource.MemorySerializer;

namespace Melville.PolyglotStats.Test.TableSource.MemorySerializer;

public class MemorySerializerTest
{
    [Fact]
    public void WriteByte()
    {
        using var writer = new MemoryWriter();
        writer.Write((byte)22);

        using var reader = new MemoryReader(writer.Name);
        reader.Read<byte>().Should().Be(22);
    }
    [Fact]
    public void Uint64()
    {
        using var writer = new MemoryWriter();
        writer.Write((ulong)22);

        using var reader = new MemoryReader(writer.Name);
        reader.Read<ulong>().Should().Be(22);
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("ab")]
    [InlineData("abdaspkgnjh]pjl ado")]
    [InlineData("a\x1234b")]
    public void WriteString(string s)
    {
        using var writer = new MemoryWriter();
        writer.WriteString(s);

        using var reader = new MemoryReader(writer.Name);
        reader.ReadString().Should().Be(s);
    }

    
}