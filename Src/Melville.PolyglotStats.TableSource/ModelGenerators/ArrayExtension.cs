using System;

namespace Melville.PolyglotStats.TableSource.ModelGenerators;

public static class ArrayExtension
{
    public static ReadOnlyMemory<char> ValueOrEmpty(this ReadOnlyMemory<char>[] row, int index) =>
        index < row.Length?row[index]:ReadOnlyMemory<char>.Empty;
}