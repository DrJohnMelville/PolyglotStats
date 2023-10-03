using System;
using System.Diagnostics;
using System.Text;
using Melville.INPC;
using Melville.PolyglotStats.TableSource.MemorySerializer;

namespace Melville.PolyglotStats.TableSource.TypeInference;

public partial class NullableWrapper : InferredType
{
    [FromConstructor] private readonly InferredType inner;

    partial void OnConstructed()
    {
        Debug.Assert(inner is not NullableWrapper);
    }

    public override bool CanParse(ReadOnlyMemory<char> datum) => inner.CanParse(datum);

    public override void WriteTypeName(StringBuilder target)
    {
        inner.WriteTypeName(target);
        target.Append('?');
    }

    public override void WriteValue(MemoryWriter writer, ReadOnlyMemory<char> value)
    {
        if (value.Length == 0)
        {
            writer.Write<byte>(0);
            return;
        }
        writer.Write<byte>(1);
        inner.WriteValue(writer, value);
    }

    public override void WriteReader(StringBuilder target)
    {
        target.Append("reader.Read<byte>()==0?default:");
        inner.WriteReader(target);
    }
}