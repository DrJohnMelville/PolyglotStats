using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Melville.INPC;
using Melville.PolyglotStats.TableSource.Parser;
using Melville.PolyglotStats.TableSource.TypeInference;

namespace Melville.PolyglotStats.TableSource.ModelGenerators;

public readonly partial struct FieldRequest
{
    [FromConstructor] private readonly ReadOnlyMemory<char> name;
    [FromConstructor] private readonly InferredType type;

    public void RenderField(StringBuilder target)
    {
        target.Append($"        ");
        type.WriteTypeName(target);
        target.Append($" {name}");
    }

    public void WriteValue(ReadOnlyMemory<char> value, StringBuilder target) =>
        type.WriteValue(value, target);
}

public readonly partial struct ModelBuilder
{
    [FromConstructor] private readonly ReadOnlyMemory<char> name;
    [FromConstructor] private readonly IList<FieldRequest> fields;

    public ModelBuilder(ReadOnlyMemory<char> name , params FieldRequest[] fields): 
        this(name, (IList<FieldRequest>)fields){}

    public void GenerateClass(StringBuilder target, IEnumerable<ReadOnlyMemory<char>[]> data)
    {
        WriteTypeDeclarationTo(target);
        WriteDataTo(target, data);
    }

    public void WriteTypeDeclarationTo(StringBuilder target)
    {
        target.AppendLine($"    public record {name}Class (");
        for (int i = 0; i < fields.Count - 1; i++)
        {
            fields[i].RenderField(target);
            target.AppendLine(",");
        }

        fields[^1].RenderField(target);
        target.AppendLine();
        target.AppendLine("    );");
    }

    public void WriteDataTo(StringBuilder target, IEnumerable<ReadOnlyMemory<char>[]> data)
    {
        target.AppendLine($"    public readonly {name}Class[] {name} = new {name}Class[] {{");
        foreach (var row in data)
        {
            target.Append("        new (");
            WriteSingleValue(0, row, target);
            for (int i = 1; i < fields.Count; i++)
            {
                target.Append(", ");
                WriteSingleValue(i, row, target);
            }
            target.AppendLine("),");
        }

        target.AppendLine("    };");
    }

    private void WriteSingleValue(int i, ReadOnlyMemory<char>[] row, StringBuilder target) => 
        fields[i].WriteValue(row.ValueOrEmpty(i), target);
}