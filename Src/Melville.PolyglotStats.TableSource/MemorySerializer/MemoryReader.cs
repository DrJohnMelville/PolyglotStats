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