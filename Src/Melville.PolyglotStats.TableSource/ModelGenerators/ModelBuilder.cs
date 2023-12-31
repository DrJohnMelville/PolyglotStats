﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Melville.INPC;
using Melville.PolyglotStats.TableSource.MemorySerializer;
using Melville.PolyglotStats.TableSource.Parser;
using Melville.PolyglotStats.TableSource.TypeInference;

namespace Melville.PolyglotStats.TableSource.ModelGenerators;

public readonly partial struct FieldRequest
{
    [FromConstructor] private readonly ReadOnlyMemory<char> name;
    [FromConstructor] private readonly InferredType type;

    public void RenderField(StringBuilder target, StringBuilder documentation)
    {
        target.Append($"        ");
        type.WriteTypeName(target);
        target.Append($" {name}");

        documentation.Append("<tr><td>");
        type.WriteTypeName(documentation);
        documentation.AppendLine($"</td><td>{name}</td><tr>");
    }

    public void WriteValue(MemoryWriter value, ReadOnlyMemory<char> target) =>
        type.WriteValue(value, target);

    public void WriteReader(StringBuilder target) => type.WriteReader(target);
}

public readonly partial struct ModelBuilder
{
    [FromConstructor] private readonly ReadOnlyMemory<char> name;
    [FromConstructor] private readonly IList<FieldRequest> fields;
    [FromConstructor] private readonly StringBuilder target;
    [FromConstructor] private readonly StringBuilder documentation;

    public IDisposable GenerateClass(StringBuilder target, IEnumerable<ReadOnlyMemory<char>[]> data)
    {
        WriteTypeDeclarationTo();
        return WriteDataTo(data);
    }

    public void WriteTypeDeclarationTo()
    {
        RenderHeaders();


        for (int i = 0; i < fields.Count - 1; i++)
        {
            fields[i].RenderField(target, documentation);
            target.AppendLine(",");
        }
        fields[^1].RenderField(target, documentation);
        RenderFooters();
    }

    private void RenderHeaders()
    {
        target.AppendLine($"    public record {name}Class (");

        documentation.AppendLine("<div><details>");
        documentation.AppendLine($"<summary>{name}</summary>");
        documentation.AppendLine(
            "<table>");
    }

    private void RenderFooters()
    {
        target.AppendLine();
        target.AppendLine("    );");

        documentation.AppendLine("</table>");
        documentation.AppendLine("</details></div>");
    }

    public IDisposable WriteDataTo(IEnumerable<ReadOnlyMemory<char>[]> data)
    {
        var file = new MemoryWriter();
        target.AppendLine($"    public readonly {name}Class[] {name} = Read{name}Class();");
        target.AppendLine($"    private static {name}Class[] Read{name}Class()");
        target.AppendLine( "    {");
        target.AppendLine($"        using var reader = new MemoryReader(\"{file.Name}\");");
        target.AppendLine($"        var ret = new {name}Class[{data.Count()}];");
        target.AppendLine($"        for (int i = 0; i < ret.Length; i++)");
        target.Append($"            ret[i] = new (");
        fields[0].WriteReader(target);
        foreach (var field in fields.Skip(1))
        {
            target.Append(", ");
            field.WriteReader(target);

        }

        target.AppendLine(");");
        target.AppendLine("        return ret;");
        target.AppendLine("        }");
    
        foreach (var row in data)
        {
            for (int i = 0; i < fields.Count; i++)
            {
                fields[i].WriteValue(file, row.ValueOrEmpty(i));
            }
        }
        return file;
    }
}