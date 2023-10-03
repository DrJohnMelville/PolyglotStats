using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Melville.FileSystem;
using Melville.INPC;
using Melville.PolyglotStats.TableSource.Parser;

namespace Melville.PolyglotStats.TableSource.ModelGenerators;

public class GeneratorFacade
{
    public static async Task<GeneratedCodeResult> QueryToCode(string code, IDiskFileSystemConnector connector) => 
        (await new TableParser(code.AsMemory(), connector).ParseAsync()).GenerateCode();
}

public sealed partial class GeneratedCodeResult : IDisposable
{
    [FromConstructor] public string Code { get; }
    [FromConstructor] private readonly IEnumerable<IDisposable> data;

    public void Dispose()
    {
        foreach (var datum in data)
        {
            datum.Dispose();
        }
    }
}