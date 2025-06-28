namespace Vertr.MarketData.Contracts;

public record class Instrument
{
    public static readonly Guid RUB = new Guid("a92e2e25-a698-45cc-a781-167cf465257c");

    public Guid Id { get; set; }
    public required Symbol Symbol { get; init; }
    public string? InstrumentType { get; init; }
    public string? Name { get; init; }
    public string? InstrumentKind { get; init; }
    public string? Currency { get; init; }
    public decimal? LotSize { get; init; }
}

