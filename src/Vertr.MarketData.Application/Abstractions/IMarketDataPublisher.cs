using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.Application.Abstractions;

public interface IMarketDataPublisher
{
    public Task Publish(Candle candle, CancellationToken cancellationToken = default);
}
