namespace Vertr.MarketData.Contracts;

public record class CandleSubscription
{
    public Guid InstrumentId { get; init; }

    public CandleInterval Interval { get; init; }
}
