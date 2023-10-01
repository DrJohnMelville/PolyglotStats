using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using Melville.FileSystem;
using Melville.INPC;

namespace Melville.PolyglotStats.TableSource.Parser;

public readonly partial struct ExcelFileReader
{
    [FromConstructor] private readonly IExcelDataReader reader;
    [FromConstructor] private readonly ParsedTableSet target;
    private readonly List<ReadOnlyMemory<char>> row = new();

    static ExcelFileReader()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public static async Task ParseAsync(IFile file, ParsedTableSet target)
    {
        await using var stream = await file.OpenRead();
        var reader = ExcelReaderFactory.CreateReader(stream);
        new ExcelFileReader(reader, target).Parse();
    }

    private void Parse()
    {
        do
        {
            ReadSingleTable();
        } while (reader.NextResult());
    }

    private void ReadSingleTable()
    {
        var table = new ParsedTable(reader.Name.AsMemory());
        if (!reader.Read()) return;
        ReadRow();
        table.AddTitles(row);
        while (reader.Read())
        {
            ReadRow();
            table.AddData(row);
        }
        target.Tables.Add(table);
    }

    private void ReadRow()
    {
        row.Clear();
        for (int i = 0; i < reader.FieldCount; i++)
        {
            row.Add(CanonicalizeValue(reader.GetValue(i)).AsMemory());
        }
    }

    private static string CanonicalizeValue(object? value) => value?.ToString()??"";
}