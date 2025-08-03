namespace Vertr.MarketData.Contracts.Interfaces;

public interface ISubscriptionsRepository
{
    public Task<CandleSubscription[]> GetAll();

    public Task<CandleSubscription?> GetById(Guid id);

    public Task<bool> Save(CandleSubscription subscription);

    public Task<int> Delete(Guid Id);
}
