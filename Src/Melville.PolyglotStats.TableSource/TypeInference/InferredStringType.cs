using System;
using System.Text;
using Melville.INPC;
using Melville.PolyglotStats.TableSource.MemorySerializer;

namespace Melville.PolyglotStats.TableSource.TypeInference;

[StaticSingleton]
public partial class InferredStringType : InferredType
{
    public override bool CanParse(ReadOnlyMemory<char> datum) => true;

    public override void WriteTypeName(StringBuilder target) =>
        target.Append("string");

    public override void WriteValue(MemoryWriter writer, ReadOnlyMemory<char> value) =>
        writer.WriteString(value.Span);

    public override void WriteReader(StringBuilder target) => 
        target.Append("reader.ReadString()");
}