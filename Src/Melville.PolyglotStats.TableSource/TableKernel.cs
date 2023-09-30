using System.Threading.Tasks;
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

    public  Task HandleAsync(SubmitCode command, KernelInvocationContext context)
    {   context.Display(new HtmlString($"""
                                 <details>
                                     <summary>Success</summary>
                                     <p>{command.Code}</p>
                                 </details>
                                 """));
        context.Display(context);
        return ((IKernelCommandHandler<SubmitCode>)innerKernel)
            .HandleAsync(new SubmitCode("""
                                        var title = "Hello World";
                                        """), context);
    }
}