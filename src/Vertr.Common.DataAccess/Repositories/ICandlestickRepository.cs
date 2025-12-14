using Vertr.Common.Contracts;

namespace Vertr.Common.DataAccess.Repositories;

public interface ICandlestickRepository
{
    public Task<bool> Clear(Guid instrumentId);
    public Task<IEnumerable<Candlestick?>> GetLast(Guid instrumentId, long maxItems = -1);
    public Task<long> Save(Guid instrumentId, Candlestick[] candles, int maxCount = 0, bool publish = true);
}