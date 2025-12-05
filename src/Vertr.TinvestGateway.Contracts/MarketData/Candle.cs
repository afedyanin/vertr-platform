namespace Vertr.TinvestGateway.Contracts.MarketData;

public record class Candle(
    Guid InstrumentId,
    DateTime TimeUtc,
    decimal Open,
    decimal Close,
    decimal High,
    decimal Low,
    long Volume)
{
    // TODO: Convert to extension property (C#14)
    public int Date => int.Parse(TimeUtc.ToString("yyMMdd"));

    // TODO: Convert to extension property (C#14)
    public int Time => int.Parse(TimeUtc.ToString("HHmmss"));
}