using System;
using System.Text;
using Melville.INPC;

namespace Melville.PolyglotStats.TableSource.TypeInference;

[StaticSingleton]
public partial class InferredDateType : InferredType
{
    public override bool CanParse(ReadOnlyMemory<char> datum) => DateTime.TryParse(datum.Span, out _);

    public override void WriteTypeName(StringBuilder target) =>
        target.Append("System.DateTime");

    public override void WriteValue(ReadOnlyMemory<char> value, StringBuilder target)
    {
        target.Append($"System.DateTime.Parse(\"{value}\")");
    }
}