namespace Vertr.Domain;
public record class HistoricCandle
{
    public DateTime TimeUtc { get; init; }

    public decimal Open { get; init; }

    public decimal Close { get; init; }

    public decimal High { get; init; }

    public decimal Low { get; init; }

    public long Volume { get; init; }

    public bool IsCompleted { get; init; }

    public int CandleSource { get; init; }
}
