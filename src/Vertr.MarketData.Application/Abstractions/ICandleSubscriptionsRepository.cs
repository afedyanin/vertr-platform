using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.Application.Abstractions;
public interface ICandleSubscriptionsRepository
{
    public Task<CandleSubscription[]> GetAll();

    public Task<CandleSubscription?> GetById(Guid id);

    public Task<CandleSubscription[]> GetBySymbol(string symbol);

    public Task<bool> Save(CandleSubscription subscription);

    public Task<bool> Delete(Guid id);
}
