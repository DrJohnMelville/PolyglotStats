using System.Collections.Generic;
using System.IO;
using Melville.PolyglotStats.Stats.PolyglotFormatting;

namespace Melville.PolyglotStats.Stats.Tables;

internal class TableFormatterSource: ITypeFormatterSource
{
    public IEnumerable<IConventionBasedFormaatter> CreateTypeFormatters()
    {
        return new[] { new TableFormatter() };
    }
}
internal class TableFormatter: IConventionBasedFormaatter
{
    public string MimeType => "text/html";

    public bool Format(object instance, TextWriter writer)
    {
        if (instance is not ICanRenderASHtml table) return false;
        writer.Write(table.RenderAsHtml());
        return true;
    }
}