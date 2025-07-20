using Vertr.MarketData.Contracts;

namespace Vertr.Strategies.Application.StrategiesImpl;
internal abstract class StrategyBase
{
    public Guid Id { get; init; }

    public virtual Task HandleMarketData(Candle candle, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
