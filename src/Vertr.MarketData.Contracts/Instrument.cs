namespace Vertr.MarketData.Contracts;
public record class Instrument
(
    Guid InstrumentId,
    string Ticker,
    string ClassCode,
    string Name,
    string Isin,
    int LotSize,
    string Currency
);
