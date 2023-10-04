using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Accord.Statistics.Analysis;
using Melville.PolyglotStats.Stats.PolyglotFormatting;
using Melville.PolyglotStats.Stats.Tables;

namespace Melville.PolyglotStats.Stats.HypothesisTesting;

[TypeFormatterSource(typeof(TableFormatterSource), PreferredMimeTypes = new[] {"text/html"})]
public sealed class LogisticRegressionImpl<T>:
    RegressionBase<T, bool, LogisticRegressionImpl<T>>, ICanRenderASHtml
{
    public LogisticRegressionImpl(IEnumerable<T> items, Func<T, bool> resultsFunc) : base(items, resultsFunc)
    {
    }

    public LogisticRegressionImpl(IEnumerable<T> items, Func<T, bool?> resultFunc) : base(items, resultFunc)
    {
    }

    public LogisticRegressionAnalysis Regress()
    {
        var analyzer = new LogisticRegressionAnalysis()
        {
            Iterations = 100,
            Inputs = GetInputNames(),
        };
        analyzer.Learn(InputData(), Filter.FilteredResult().Select(i=>resultsFunc(i)?1.0:0.0).ToArray());
        return analyzer;
    }

    public string RenderAsHtml()
    {
        var result = Regress();
        return 
            new XElement("div",
                new XElement("table",
                    new XElement("tr", new XElement("td", "N"), new XElement("td", result.NumberOfSamples)),
                    new XElement("tr", new XElement("td", "Chi Square"), new XElement("td", result.ChiSquare.Statistic)),
                    new XElement("tr", new XElement("td", "Degrees of Freedom"), new XElement("td", result.ChiSquare.DegreesOfFreedom)),
                    new XElement("tr", new XElement("td", "P Value"), new XElement("td", result.ChiSquare.PValue.ToString("0.0000")))
                ),
                new XElement("table",
                    new XElement("tr", new XElement("th","Name"),new XElement("th","Odds Ratio"),new XElement("th","P Value"),
                        new XElement("th","Confidence Interval")),
                    result.Coefficients.Skip(1).Select(i=>new XElement("tr", new XElement("td", i.Name), new XElement("td", i.OddsRatio.ToString("####0.##")),
                        new XElement("td", i.Wald.PValue.ToString("0.0000")), 
                        new XElement("td", $"{i.ConfidenceLower:###0.##} - {i.ConfidenceUpper:###0.##}"))
                    )
                )).ToString();
    }
}