namespace Vertr.MarketData.Contracts.Interfaces;

public interface ICandlesHistoryRepository
{
    public Task<CandlesHistoryItem[]> Get(Guid instrumentId);

    public Task<CandlesHistoryItem?> GetById(Guid id);

    public Task<bool> Save(CandlesHistoryItem item);

    public Task<int> Delete(Guid Id);

    public Task<int> DeleteAll(Guid instrumentId, DateOnly dayBefore);
}
