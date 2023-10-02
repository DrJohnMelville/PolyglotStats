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
        var code = await GeneratorFacade.QueryToCode(command.Code, new DiskFileSystemConnector());
        context.DisplayCollapsed("Source Code", code);
        await ExecuteCSharpCode(context, code);
        context.Display("done");
    }

    private Task ExecuteCSharpCode(KernelInvocationContext context, string code) =>
        ((IKernelCommandHandler<SubmitCode>)innerKernel)
        .HandleAsync(new SubmitCode(code), context);
}