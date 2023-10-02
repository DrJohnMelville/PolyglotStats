using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using FSharp.Compiler.Syntax;
using Melville.PolyglotStats.TableSource.ModelGenerators;

namespace Melville.PolyglotStats.TableSource.Parser;

public class ParsedTableSet
{
    public ReadOnlyMemory<char> Name { get; set; }= "Data".AsMemory();
    public List<ParsedTable> Tables = new List<ParsedTable>();
    private readonly StringBuilder target = new();

    public string GenerateCode()
    {
        var target = new StringBuilder();
        GenerateCodeTo(target);
        return target.ToString();
    }

    private void GenerateCodeTo(StringBuilder target)
    {
        GenerateClassHeader(target);
        GenerateTables(target);
        GenerateClassFooter(target);
    }

    private void GenerateClassHeader(StringBuilder target) => 
        target.AppendLine($"public class {Name}Class {{");

    private void GenerateClassFooter(StringBuilder target)
    {
        target.AppendLine("}");
        target.Append($"public readonly {Name}Class {Name} = new();");
    }

    private void GenerateTables(StringBuilder target)
    {
        foreach (var table in Tables)
        {
            new ModelBuilder(table.Name.CanonicalName(), table.FieldRequests())
                .GenerateClass(target, table.Rows);
        }
    }
}