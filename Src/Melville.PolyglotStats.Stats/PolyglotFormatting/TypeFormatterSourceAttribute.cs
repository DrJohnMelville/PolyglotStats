using System;
using System.Collections.Generic;
using System.IO;

namespace Melville.PolyglotStats.Stats.PolyglotFormatting;

[AttributeUsage(AttributeTargets.Class)]
internal class TypeFormatterSourceAttribute : Attribute
{
    public TypeFormatterSourceAttribute(Type formatterSourceType)
    {
        FormatterSourceType = formatterSourceType;
    }

    public Type FormatterSourceType { get; }

    public string[] PreferredMimeTypes { get; set; } = Array.Empty<string>();
}

internal interface ITypeFormatterSource
{
    IEnumerable<IConventionBasedFormaatter> CreateTypeFormatters();
}

internal interface IConventionBasedFormaatter
{
    string MimeType { get; }

    public bool Format(object instance, TextWriter writer);
}