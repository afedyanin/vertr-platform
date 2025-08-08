namespace Vertr.MarketData.Contracts;

public record class Instrument
{
    public Guid Id { get; set; }
    public required Symbol Symbol { get; set; }
    public string? InstrumentType { get; set; }
    public string? Name { get; set; }
    public string? Currency { get; set; }
    public decimal? LotSize { get; set; }

    public string GetFullName() => $"{Symbol.ClassCode}.{Symbol.Ticker} ({Name})";
}

