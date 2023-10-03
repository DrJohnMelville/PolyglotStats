using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;

namespace Melville.PolyglotStats.TableSource.MemorySerializer;

public sealed class MemoryWriter: IDisposable
{
    public string Name { get; } = Guid.NewGuid().ToString();
    private readonly MemoryMappedFile mmf;
    private readonly Stream stream;

    public const long maxSize = 1024 * 1024 * 1024;

    public MemoryWriter()
    {
        mmf = MemoryMappedFile.CreateNew(Name, maxSize);
        stream = mmf.CreateViewStream(0, maxSize);
    }

    public void Dispose()
    {
        stream.Dispose();
        mmf.Dispose();
    }

    public void Write<T>(T value) where T : struct
    {
        var t = MemoryMarshal.Cast<T, byte>(MemoryMarshal.CreateSpan(ref value, 1));
        stream.Write(t);
    }

    public void WriteString(string value)
    {
        var length = Encoding.UTF8.GetByteCount(value);
        Write(length);
        Span<byte> buffer = stackalloc byte[length];
        Encoding.UTF8.GetBytes(value, buffer);
        stream.Write(buffer);
    }
}