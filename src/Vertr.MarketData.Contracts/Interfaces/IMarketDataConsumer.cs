namespace Vertr.MarketData.Contracts.Interfaces;

public interface IMarketDataConsumer
{
    public Task Consume(
        Func<Candle, CancellationToken, Task> handler,
        CancellationToken cancellationToken = default);
}
