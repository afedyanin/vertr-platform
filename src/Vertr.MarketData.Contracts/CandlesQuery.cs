namespace Vertr.MarketData.Contracts;

public record class CandlesQuery
(
    string Symbol,
    CandleInterval Interval,
    CandleSource CandleSource = CandleSource.Tinvest,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    long? MaxItems = 1000
);
