namespace Vertr.MarketData.Contracts;

public record class Candle(
    Guid InstrumentId,
    DateTime TimeUtc,
    decimal Open,
    decimal Close,
    decimal High,
    decimal Low,
    long Volume);
