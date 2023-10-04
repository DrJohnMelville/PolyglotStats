using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Accord.Statistics.Analysis;
using Melville.PolyglotStats.Stats.PolyglotFormatting;
using Melville.PolyglotStats.Stats.Tables;

namespace Melville.PolyglotStats.Stats.HypothesisTesting;

[TypeFormatterSource(typeof(TableFormatterSource), PreferredMimeTypes = new[] {"text/html"})]
public sealed class LinearRegressionImpl<T> : RegressionBase<T, double, LinearRegressionImpl<T>>, ICanRenderASHtml
{
    public LinearRegressionImpl(IEnumerable<T> items, Func<T, double> resultsFunc) : base(items, resultsFunc)
    {
    }

    public LinearRegressionImpl(IEnumerable<T> items, Func<T, double?> resultFunc) : base(items, resultFunc)
    {
    }

    public MultipleLinearRegressionAnalysis Regress()
    {
        var analyzer = new MultipleLinearRegressionAnalysis(true)
        {
            Inputs = GetInputNames()
        };
        analyzer.Learn(InputData(), Filter.FilteredResult().Select(resultsFunc).ToArray());
        return analyzer;
    }

    public string RenderAsHtml()
    {
        var result = Regress();
        return new XElement("div",
            new XElement("table",
                new XElement("tr", new XElement("td", "N"), new XElement("td", result.NumberOfSamples)),
                new XElement("tr", new XElement("td", "Adjusted R\x00B2"), new XElement("td", result.RSquareAdjusted.ToString("0.0000"))),
                new XElement("tr", new XElement("td", "F Test (p)"), new XElement("td", result.FTest.Statistic.ToString("0.0000")), new XElement("td", result.FTest.PValue.ToString("0.0000"))),
                new XElement("tr", new XElement("td", "Z Test (p)"), new XElement("td", result.ZTest.Statistic.ToString("0.0000")), new XElement("td", result.ZTest.PValue.ToString("0.0000"))),
                new XElement("tr", new XElement("td", "Chi Squared Test (p)"), new XElement("td", result.ChiSquareTest.Statistic.ToString("0.0000")), new XElement("td", result.ChiSquareTest.PValue.ToString("0.0000")))
          ),
            new XElement("table",
                new XElement("tr", new XElement("th","Name"),new XElement("th","Coefficient"),new XElement("th","T Statistic"),new XElement("th","P Value"),
                    new XElement("th","Confidence Interval")),
                result.Coefficients.Select(i=>new XElement("tr", new XElement("td", i.Name), new XElement("td", i.Value.ToString("####0.##")),
                    new XElement("td", i.TTest.Statistic.ToString("0.0000")), new XElement("td", i.TTest.PValue.ToString("0.0000")), 
                    new XElement("td", $"{i.ConfidenceLower:###0.##} - {i.ConfidenceUpper:###0.##}"))
                )
            )).ToString();
    }
}