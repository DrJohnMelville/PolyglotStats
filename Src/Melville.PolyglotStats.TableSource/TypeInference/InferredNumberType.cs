using System;
using System.Globalization;
using System.Numerics;
using Melville.INPC;

namespace Melville.PolyglotStats.TableSource.TypeInference;

[FromConstructor]
public partial class InferredNumberType<T> : InferredType where T : INumberBase<T>
{
    public static readonly InferredNumberType<T> Instance = new InferredNumberType<T>();

    public override bool CanParse(ReadOnlyMemory<char> datum) => T.TryParse(datum.Span, NumberStyles.Any, null, out _);
}