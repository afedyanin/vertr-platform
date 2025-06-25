namespace Vertr.MarketData.Contracts;

public record class InstrumentIdentity(
    string ClassCode,
    string Ticker,
    Guid? InstrumentId = null,
    string? Isin = null);
