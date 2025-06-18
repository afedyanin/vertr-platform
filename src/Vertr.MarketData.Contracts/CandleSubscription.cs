namespace Vertr.MarketData.Contracts;

public record class CandleSubscription
{
    public required string ClassCode { get; init; }

    public required string Symbol { get; init; }

    public CandleInterval CandleInterval { get; init; }
}
