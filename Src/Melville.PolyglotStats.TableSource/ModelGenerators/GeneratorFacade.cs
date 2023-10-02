using System;
using System.Threading.Tasks;
using Melville.FileSystem;
using Melville.PolyglotStats.TableSource.Parser;

namespace Melville.PolyglotStats.TableSource.ModelGenerators;

public class GeneratorFacade
{
    public static async Task<string> QueryToCode(string code, IDiskFileSystemConnector connector) => 
        (await new TableParser(code.AsMemory(), connector).ParseAsync()).GenerateCode();
}