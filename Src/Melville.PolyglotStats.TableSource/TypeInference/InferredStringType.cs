using System;
using System.Text;
using Melville.INPC;

namespace Melville.PolyglotStats.TableSource.TypeInference;

[StaticSingleton]
public partial class InferredStringType : InferredType
{
    public override bool CanParse(ReadOnlyMemory<char> datum) => true;

    public override void WriteTypeName(StringBuilder target) =>
        target.Append("string");
}