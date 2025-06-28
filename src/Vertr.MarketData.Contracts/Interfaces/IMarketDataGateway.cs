namespace Vertr.MarketData.Contracts.Interfaces;
public interface IMarketDataGateway
{
    public Task<Instrument[]?> FindInstrument(string query);

    public Task<Instrument?> GetInstrument(Symbol instrumentIdentity);

    public Task<Instrument?> GetInstrument(Guid instrumentId);

    public Task<Candle[]?> GetCandles(
        Symbol instrumentIdentity,
        CandleInterval interval,
        DateTime from,
        DateTime to,
        int? limit);
}
