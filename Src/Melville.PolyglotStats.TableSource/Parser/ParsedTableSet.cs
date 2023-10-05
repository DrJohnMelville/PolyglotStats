using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using FSharp.Compiler.Syntax;
using Melville.PolyglotStats.TableSource.MemorySerializer;
using Melville.PolyglotStats.TableSource.ModelGenerators;

namespace Melville.PolyglotStats.TableSource.Parser;

public class ParsedTableSet
{
    public ReadOnlyMemory<char> Name { get; set; }= "Data".AsMemory();
    public List<ParsedTable> Tables = new List<ParsedTable>();
    private readonly StringBuilder target = new();
    private readonly StringBuilder documentation = new(); 

    public GeneratedCodeResult GenerateCode()
    {
        var streams = GenerateCodeTo();
        return new GeneratedCodeResult(target.ToString(), documentation.ToString(), streams);
    }

    private IEnumerable<IDisposable> GenerateCodeTo()
    {
        GenerateUsings();
        GenerateClassHeader();
        GenerateReaderDefinition();
        var ret = GenerateTables();
        GenerateClassFooter();
        return ret;
    }

    private void GenerateReaderDefinition()
    {
        target.AppendLine(ReaderSource.Code());
    }

    private void GenerateUsings()
    {
        target.AppendLine("""
                          #define InsideGeneratedCode
                          using System;
                          using System.IO;
                          using System.IO.MemoryMappedFiles;
                          using System.Runtime.InteropServices;
                          using System.Text;
                          
                          """);
    }

    private void GenerateClassHeader()
    {
        target.AppendLine($"public class {Name}Class {{");
    }

    private void GenerateClassFooter()
    {
        target.AppendLine("}");
        target.Append($"public readonly {Name}Class {Name} = new();");

    }

    private IEnumerable<IDisposable> GenerateTables() => 
        Tables.Select(GenerateSingleTable).ToArray();

    private IDisposable GenerateSingleTable(ParsedTable table)
    {
        return new ModelBuilder(table.Name.CanonicalName(), table.FieldRequests(), target, documentation)
            .GenerateClass(target, table.Rows);
    }
}