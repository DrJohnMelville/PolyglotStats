using System;
using Melville.INPC;

namespace Melville.PolyglotStats.TableSource.TypeInference;

[StaticSingleton]
public partial class InferredDateType : InferredType
{
    public override bool CanParse(ReadOnlyMemory<char> datum) => DateTime.TryParse(datum.Span, out _);
}