using Vertr.MarketData.Contracts;

namespace Vertr.TinvestGateway.Contracts.Interfaces;

public interface ITinvestGatewayMarketData
{
    public Task<Instrument[]?> FindInstrument(string query);

    public Task<Instrument?> GetInstrumentByTicker(string classCode, string ticker);

    public Task<Instrument?> GetInstrumentById(string instumentId);

    public Task<Candle[]?> GetCandles(
        InstrumentIdentity instrumentIdentity,
        CandleInterval interval,
        DateTime from,
        DateTime to,
        int? limit);
}
