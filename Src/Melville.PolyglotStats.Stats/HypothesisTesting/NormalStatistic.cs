using Accord.Statistics.Distributions.Univariate;

namespace Melville.PolyglotStats.Stats.HypothesisTesting;

public class NormalStatistic : Statistic
{
    public double Mean { get; }
    public double StdDeviation { get; }
    public double ZScore => innerValue;

    public NormalStatistic(double mean, double stdDeviation, double value) : base(new NormalDistribution(mean, stdDeviation), value)
    {
        Mean = mean;
        StdDeviation = stdDeviation;
    }

    public override string ToString() => $"T: {ZScore:0.0###} p = {TwoTailedP:0.0##}";
}