using Vertr.Common.Contracts;

namespace Vertr.Common.DataAccess.Repositories;

public interface IMarketTradeRepository
{
    public Task Clear();
    public Task<IEnumerable<MarketTrade>> GetAll();
    public Task<MarketTrade?> Get(Guid instrumentId);
    public Task Save(MarketTrade marketTrade);
}
