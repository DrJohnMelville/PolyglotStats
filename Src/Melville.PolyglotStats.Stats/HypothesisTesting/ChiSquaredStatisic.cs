using Accord.Statistics.Distributions.Univariate;

namespace Melville.PolyglotStats.Stats.HypothesisTesting;

public class ChiSquaredStatisic 
{
    public double ChiSquared { get; }
    public int DegreesOfFreedom { get; }
    public double P => 1.0 - 
                       new ChiSquareDistribution(DegreesOfFreedom)
                           .DistributionFunction(ChiSquared);

    public ChiSquaredStatisic(double innerValue, int freedom)
    {
        ChiSquared = innerValue;
        DegreesOfFreedom = freedom;
    }
}