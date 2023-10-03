using System;
using System.Formats.Asn1;
using System.Globalization;
using System.Numerics;
using System.Text;
using Melville.INPC;
using Melville.PolyglotStats.TableSource.MemorySerializer;

namespace Melville.PolyglotStats.TableSource.TypeInference;

[FromConstructor]
public partial class InferredNumberType<T> : InferredType where T : struct, INumberBase<T>
{
    public static readonly InferredNumberType<T> Instance = new InferredNumberType<T>();

    public override bool CanParse(ReadOnlyMemory<char> datum) =>
        T.TryParse(datum.Span, NumberStyles.Any, null, out _);

    public override void WriteTypeName(StringBuilder target) =>
        target.Append(typeof(T));

    public override void WriteValue(MemoryWriter writer, ReadOnlyMemory<char> value) => 
        writer.Write(T.Parse(value.Span, NumberStyles.Any, CultureInfo.InstalledUICulture));
}