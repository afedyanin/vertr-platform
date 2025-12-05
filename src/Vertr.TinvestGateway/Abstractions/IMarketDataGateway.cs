using Vertr.TinvestGateway.Contracts.MarketData;

namespace Vertr.TinvestGateway.Abstractions;

public interface IMarketDataGateway
{
    public Task<Instrument[]?> FindInstrument(string query);

    public Task<Instrument?> GetInstrumentBySymbol(string classCode, string ticker);

    public Task<Instrument?> GetInstrumentById(Guid instrumentId);

    public Task<Candle[]?> GetCandles(
        Guid instrumentId,
        DateOnly? date = null,
        CandleInterval interval = CandleInterval.Min_1);
}