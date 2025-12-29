namespace Vertr.Common.Contracts;

public class ThresholdSettings
{
    public double ThresholdValue { get; set; } = 0.0001;

    public int ThresholdSigma { get; set; } = 1;

    public bool UseStatsThreshold { get; set; }
}
