using System;
using System.Text;
using Melville.INPC;
using Melville.PolyglotStats.TableSource.MemorySerializer;

namespace Melville.PolyglotStats.TableSource.TypeInference;

[StaticSingleton]
public partial class InferredDateType : InferredType
{
    public override bool CanParse(ReadOnlyMemory<char> datum) => DateTime.TryParse(datum.Span, out _);

    public override void WriteTypeName(StringBuilder target) =>
        target.Append("System.DateTime");

    public override void WriteValue(MemoryWriter writer, ReadOnlyMemory<char> value) => 
        writer.Write(DateTime.Parse(value.Span));
}