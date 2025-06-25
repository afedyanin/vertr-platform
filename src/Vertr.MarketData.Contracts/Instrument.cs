namespace Vertr.MarketData.Contracts;

public record class Instrument(
    InstrumentIdentity InstrumentIdentity,
    string? InstrumentType,
    string? Name,
    string? InstrumentKind,
    string? Currency,
    decimal? LotSize
    );
