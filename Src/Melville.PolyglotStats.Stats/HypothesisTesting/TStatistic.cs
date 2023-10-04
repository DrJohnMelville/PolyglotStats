using Accord.Statistics.Distributions.Univariate;

namespace Melville.PolyglotStats.Stats.HypothesisTesting;

public class TStatistic:Statistic
{
    public double T => innerValue;
    public double DegreesOfFreedom { get; }

    public TStatistic(double t, double degreesOfFreedom):base(new TDistribution(degreesOfFreedom), t)
    {
        DegreesOfFreedom = degreesOfFreedom;
    }
}