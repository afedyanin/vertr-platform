using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.Application.Abstractions;
public interface IMarketInstrumentsRepository
{
    public Task<MarketInstrument[]> GetAll();

    public Task<MarketInstrument?> GetById(Guid id);

    public Task<MarketInstrument[]> GetBySymbol(string symbol);

    public Task<bool> Save(MarketInstrument instrument);

    public Task<bool> Delete(Guid id);
}
