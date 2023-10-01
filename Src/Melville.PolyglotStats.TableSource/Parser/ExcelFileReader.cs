using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using Melville.FileSystem;
using Melville.INPC;
using Microsoft.CodeAnalysis.VisualBasic;

namespace Melville.PolyglotStats.TableSource.Parser;

public readonly partial struct ExcelFileReader
{
    [FromConstructor] private readonly ParsedTableSet target;
    private readonly List<ReadOnlyMemory<char>> row = new();

    static ExcelFileReader()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public async Task ParseAsync(IFile file)
    {
        await using var stream = await file.OpenRead();
        var reader = ExcelReaderFactory.CreateReader(stream);
        Parse(reader);
    }

    private void Parse(IExcelDataReader reader)
    {
        do
        {
            ReadSingleTable(reader);
        } while (reader.NextResult());
    }

    private void ReadSingleTable(IExcelDataReader reader)
    {
        var table = new ParsedTable(reader.Name.AsMemory());
        if (!reader.Read()) return;
        ReadRow(reader);
        table.AddTitles(row);
        while (reader.Read())
        {
            ReadRow(reader);
            table.AddData(row);
        }
        target.Tables.Add(table);
    }

    private void ReadRow(IExcelDataReader reader)
    {
        row.Clear();
        for (int i = 0; i < reader.FieldCount; i++)
        {
            row.Add(CanonicalizeValue(reader.GetValue(i)).AsMemory());
        }
    }

    private static string CanonicalizeValue(object? value) => value?.ToString()??"";
}