using System;
using System.Diagnostics;
using System.Text;
using Melville.INPC;

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
}