using Vertr.Domain.Enums;

namespace Vertr.Domain.Settings;
public record class StrategySettings
{
    public required string Symbol { get; init; }

    public CandleInterval Interval { get; init; } = CandleInterval._10Min;

    public required PredictorType PredictorType { get; init; }

    public required Sb3Algo Sb3Algo { get; init; }

    public int QuantityLots { get; init; }
}
