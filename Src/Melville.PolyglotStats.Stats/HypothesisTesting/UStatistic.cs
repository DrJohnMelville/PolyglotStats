using System;
using Accord.Statistics.Distributions.Univariate;

namespace Melville.PolyglotStats.Stats.HypothesisTesting;

public class UStatistic : Statistic
{
    public double UMax { get; }
    public double UMin { get; }
    public double ZScore => innerValue;
    public UStatistic(double u1, double u2, double zScore) : 
        base(new NormalDistribution(0, 1), zScore)
    {
        UMax = Math.Max(u1, u2);
        UMin = Math.Min(u1, u2);
    }
}