using Vertr.Domain.Enums;

namespace Vertr.Domain.Settings;
public record class StrategySettings
{
    public required string Symbol { get; set; }

    public required CandleInterval Interval { get; set; } = CandleInterval._10Min;

    public required PredictorType PredictorType { get; set; }

    public required Sb3Algo Sb3Algo { get; set; }
}
