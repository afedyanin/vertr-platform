namespace Vertr.MarketData.Contracts.Interfaces;

public interface ICandlesRepository
{
    public Task<IEnumerable<Candle>> Get(
        Guid instrumentId,
        int? limit = 100,
        CancellationToken cancellationToken = default);

    public Task<Candle?> GetLast(Guid instrumentId);

    public Task<int> Upsert(IEnumerable<Candle> candles);

    public Task<int> Delete(Guid instrumentId, DateTime timeBefore);

}
