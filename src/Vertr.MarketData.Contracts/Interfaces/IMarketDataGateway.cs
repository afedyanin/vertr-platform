namespace Vertr.MarketData.Contracts.Interfaces;
public interface IMarketDataGateway
{
    public Task<Instrument[]?> FindInstrument(string query);

    public Task<Instrument?> GetInstrument(InstrumentIdentity instrumentIdentity);

    public Task<Candle[]?> GetCandles(
        InstrumentIdentity instrumentIdentity,
        CandleInterval interval,
        DateTime from,
        DateTime to,
        int? limit);
}
