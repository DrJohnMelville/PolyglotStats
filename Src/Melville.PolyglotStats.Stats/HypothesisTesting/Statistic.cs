using System;
using Accord.Statistics.Distributions;

namespace Melville.PolyglotStats.Stats.HypothesisTesting;

public class Statistic
{
    protected readonly IUnivariateDistribution Distribution;
    protected readonly double innerValue;
    public Statistic(IUnivariateDistribution distribution, double innerValue)
    {
        Distribution = distribution;
        this.innerValue = innerValue;
    }
    public double OneSidedLessP => Distribution.DistributionFunction(innerValue);
    public double OneSidedGreaterP => 1.0 - OneSidedLessP;

    public double TwoTailedP
    {
        get
        {
            var absT = Math.Abs(innerValue);
            return Distribution.DistributionFunction(-absT) + (1 - Distribution.DistributionFunction(absT));
        }
    }
}