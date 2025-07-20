namespace Vertr.MarketData.Contracts;

public record class Candle(
    DateTime TimeUtc,
    decimal Open,
    decimal Close,
    decimal High,
    decimal Low,
    long Volume,
    Guid instrumentId
);
