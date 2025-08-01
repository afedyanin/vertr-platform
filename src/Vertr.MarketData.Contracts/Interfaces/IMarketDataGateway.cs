namespace Vertr.MarketData.Contracts.Interfaces;
public interface IMarketDataGateway
{
    public Task<Instrument[]?> FindInstrument(string query);

    public Task<Instrument?> GetInstrumentBySymbol(Symbol instrumentIdentity);

    public Task<Instrument?> GetInstrumentById(Guid instrumentId);
}
