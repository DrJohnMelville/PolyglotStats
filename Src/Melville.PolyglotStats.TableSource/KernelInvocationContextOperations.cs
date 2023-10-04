using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.DotNet.Interactive;

namespace Melville.PolyglotStats.TableSource;
public static class KernelInvocationContextOperations
{
    public static void DisplayCollapsed(this KernelInvocationContext context, string title, string body)
    {
        context.Display(new HtmlString($"""
                                        <details>
                                            <summary>{title}</summary>
                                        <pre>
                                        {body}
                                        </pre>
                                        </details>
                                        """));
    }

}
