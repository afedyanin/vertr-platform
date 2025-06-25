namespace Vertr.MarketData.Contracts;

public record class CandleSubscription
{
    public required InstrumentIdentity InstrumentIdentity { get; init; }

    public CandleInterval Interval { get; init; }
}
