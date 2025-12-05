namespace Vertr.CommandLine.Models;

public record class Candle
{
    public DateTime TimeUtc { get; init; }
    public decimal Open { get; init; }
    public decimal Close { get; init; }
    public decimal High { get; init; }
    public decimal Low { get; init; }
    public long Volume { get; init; }
}

public enum PriceType
{
    Open,
    Close,
    Mid, // => Open + Close / 2
    Avg
}