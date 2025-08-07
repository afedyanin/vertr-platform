namespace Vertr.MarketData.Contracts;
public record class CandlesHistoryItem
{
    public Guid Id { get; init; }

    public Guid InstrumentId { get; set; }

    public CandleInterval Interval { get; set; }

    public DateOnly Day { get; set; }

    public byte[] Data { get; set; } = [];

    public long Count { get; set; }
}
