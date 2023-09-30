using System.Linq;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;

namespace Melville.PolyglotStats.TableSource;

public class KernelConnector
{
    public static void AddKernelToRoot()
    {
        if (KernelInvocationContext.Current?.HandlingKernel.RootKernel is CompositeKernel ck)
        {
            AddKernelTo(ck);
        }
    }

    private static void AddKernelTo(CompositeKernel parentKernel)
    {
        var rootKernel = parentKernel.ChildKernels.OfType<CSharpKernel>().First();
        parentKernel.Add(new TableKernel(rootKernel));
    }
}
