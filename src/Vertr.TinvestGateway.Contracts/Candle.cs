namespace Vertr.TinvestGateway.Contracts;
public record class Candle(
    DateTime TimeUtc,
    string Symbol,
    CandleInterval Interval,
    decimal Open,
    decimal Close,
    decimal High,
    decimal Low,
    long Volume,
    bool? IsCompleted
);
