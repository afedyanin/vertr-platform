namespace Vertr.TinvestGateway.Contracts;

public record class Candle(
    string Id,
    DateTime TimeUtc,
    decimal Open,
    decimal Close,
    decimal High,
    decimal Low,
    long Volume
);
