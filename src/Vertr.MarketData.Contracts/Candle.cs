namespace Vertr.MarketData.Contracts;
public record class Candle(
    Guid Id,
    DateTime TimeUtc,
    string Symbol,
    CandleInterval Interval,
    CandleSource Source,
    decimal Open,
    decimal Close,
    decimal High,
    decimal Low,
    long Volume
    );
