﻿using System;
using System.Text;
using Melville.INPC;
using Melville.PolyglotStats.TableSource.MemorySerializer;

namespace Melville.PolyglotStats.TableSource.TypeInference;

[StaticSingleton]
public partial class InferredBooleanType : InferredType
{
    private static readonly string[] TrueValues =
    {
        "yes", "y", "true", "t", "present", "positive", "1"
    };
    private static readonly string[] FalseValues =
    {
        "no", "n", "false", "f", "absent", "negative", "0"
    };

    public override bool CanParse(ReadOnlyMemory<char> datum)
    {
        var datumSpan = datum.Span;
        return CheckSpanAgainstValues(datumSpan, TrueValues) || CheckSpanAgainstValues(datumSpan, FalseValues);
    }

    private static bool CheckSpanAgainstValues(ReadOnlySpan<char> datumSpan, string[] values)
    {
        foreach (var value in values)
        {
            if (value.AsSpan().Equals(datumSpan, StringComparison.CurrentCultureIgnoreCase)) return true;
        }
        return false;
    }

    public override void WriteTypeName(StringBuilder target) => target.Append("bool");

    public override void WriteValue(MemoryWriter writer, ReadOnlyMemory<char> value) =>
        writer.Write(CheckSpanAgainstValues(value.Span, TrueValues));
}