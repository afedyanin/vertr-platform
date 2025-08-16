namespace Vertr.MarketData.Contracts.Interfaces;
public interface ICandlesHistoryLoader
{
    public Task<CandlesHistoryItem?> GetCandlesHistory(Guid instrumentId, DateOnly day, bool force = false);
}


