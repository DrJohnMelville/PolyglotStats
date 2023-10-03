using System.Threading.Tasks;
using Melville.FileSystem;
using Melville.PolyglotStats.TableSource.ModelGenerators;
using Microsoft.AspNetCore.Html;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.CSharp;

namespace Melville.PolyglotStats.TableSource;

public class TableKernel: Kernel, IKernelCommandHandler<SubmitCode>
{
    private readonly CSharpKernel innerKernel;
    public TableKernel(CSharpKernel innerKernel) : base("read-tables")
    {
        this.innerKernel = innerKernel;
    }

    public async Task HandleAsync(SubmitCode command, KernelInvocationContext context)
    {
        using var result = await GeneratorFacade.QueryToCode(command.Code, new DiskFileSystemConnector());
           //Holding on to result until after ExecuteCSharpCode keeps the memory mapped filed open
        context.DisplayCollapsed("Source Code", result.Code);
        await ExecuteCSharpCode(context, result.Code);
        context.Display("done");
    }

    private Task ExecuteCSharpCode(KernelInvocationContext context, string code) =>
        ((IKernelCommandHandler<SubmitCode>)innerKernel)
        .HandleAsync(new SubmitCode(code), context);
}