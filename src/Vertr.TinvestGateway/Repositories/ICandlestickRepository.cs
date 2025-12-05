using Vertr.TinvestGateway.Contracts.MarketData;

namespace Vertr.TinvestGateway.Repositories;

public interface ICandlestickRepository
{
    public Task<bool> Clear(Guid instrumentId);
    public Task<IEnumerable<Candlestick?>> GetLast(Guid instrumentId, long maxItems = -1);
    public Task<long> Save(Guid instrumentId, Candlestick[] candles, int maxCount = 0);
}