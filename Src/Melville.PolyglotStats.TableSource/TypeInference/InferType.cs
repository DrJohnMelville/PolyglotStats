using System;
using System.Collections.Generic;
using System.Linq;

namespace Melville.PolyglotStats.TableSource.TypeInference;

public readonly struct InferType
{
    public static InferredType[] TypeLibrary =
    {
        InferredNumberType<double>.Instance,
        InferredNumberType<ulong>.Instance,
        InferredNumberType<uint>.Instance,
        InferredNumberType<long>.Instance,
        InferredNumberType<int>.Instance,
        InferredBooleanType.Instance,
        InferredDateType.Instance,
    };

    private readonly List<InferredType> candidates = TypeLibrary.ToList();

    public InferType()
    {
    }

    public static InferredType Of(IEnumerable<ReadOnlyMemory<char>> data)
    {
        return new InferType().Select(data);
    }

    private InferredType Select(IEnumerable<ReadOnlyMemory<char>> data)
    {
        bool nullable = false;
        foreach (var datum in data)
        {
            if (IsNullValue(datum))
                nullable = true;
            else
                FilterList(datum);
            if (candidates.Count == 0) break;
        }
        return candidates.DefaultIfEmpty(InferredStringType.Instance).Last().SelectByNullability(nullable);
    }

    private static bool IsNullValue(ReadOnlyMemory<char> datum) => datum.Length == 0;

    private void FilterList(ReadOnlyMemory<char> datum)
    {
        for (int i = candidates.Count -1; i >= 0; i--)
        {
                FilterCandidateList(datum, i);
        }
    }

    private void FilterCandidateList(ReadOnlyMemory<char> datum, int i)
    {
        if (!candidates[i].CanParse(datum))
            candidates.RemoveAt(i);
    }
}