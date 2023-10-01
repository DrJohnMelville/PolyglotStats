using System;
using Melville.INPC;

namespace Melville.PolyglotStats.TableSource.TypeInference;

[StaticSingleton]
public partial class InferredStringType : InferredType
{
    public override bool CanParse(ReadOnlyMemory<char> datum) => true;
}