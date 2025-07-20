namespace Vertr.MarketData.Contracts.Interfaces;
public interface IMarketDataPublisher
{
    public Task Publish(Candle candle, CancellationToken cancellationToken = default);
}
