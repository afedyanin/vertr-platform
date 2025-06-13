namespace Vertr.TinvestGateway.Contracts;
public record class Instrument(
    string? Isin,
    string? Ticker,
    string? ClassCode,
    string? InstrumentType,
    string? Name,
    string? Uid,
    string? InstrumentKind,
    string? Currency,
    decimal? LotSize
    );
