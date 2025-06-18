using Vertr.MarketData.Contracts;

namespace Vertr.TinvestGateway.Contracts.Interfaces;

public interface ITinvestGatewayMarketData
{
    public Task<Instrument[]?> FindInstrument(string query);

    public Task<Instrument?> GetInstrumentByTicker(string ticker, string classCode);

    public Task<Instrument?> GetInstrumentById(string instumentId);

    public Task<Candle[]?> GetCandles(
        string ticker, string classCode,
        CandleInterval interval,
        DateTime from,
        DateTime to,
        int? limit);
}
