using Vertr.Domain.Enums;

namespace Vertr.Domain.Settings;
public record class StrategySettings
{
    public required string Symbol { get; set; }

    public required CandleInterval Interval { get; set; }

    public required string PredictorType { get; set; }

    public required string Sb3Algo { get; set; }
}
