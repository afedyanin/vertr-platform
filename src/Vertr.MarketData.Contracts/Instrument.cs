namespace Vertr.MarketData.Contracts;

public record class Instrument
{
    public required InstrumentIdentity InstrumentIdentity { get; init; }
    public string? InstrumentType { get; init; }
    public string? Name { get; init; }
    public string? InstrumentKind { get; init; }
    public string? Currency { get; init; }
    public decimal? LotSize { get; init; }
} 
