using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Melville.INPC;
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
}

public readonly partial struct ModelBuilder
{
    [FromConstructor] private readonly ReadOnlyMemory<char> name;
    [FromConstructor] private readonly IList<FieldRequest> fields;

    public ModelBuilder(ReadOnlyMemory<char> name , params FieldRequest[] fields): 
        this(name, (IList<FieldRequest>)fields){}

    public void WriteTypeDeclarationTo(StringBuilder target)
    {
        target.AppendLine($"    public partial record {name} (");
        for (int i = 0; i < fields.Count - 1; i++)
        {
            fields[i].RenderField(target);
            target.AppendLine(",");
        }

        fields[^1].RenderField(target);
        target.AppendLine();
        target.AppendLine("    );");
    }
}