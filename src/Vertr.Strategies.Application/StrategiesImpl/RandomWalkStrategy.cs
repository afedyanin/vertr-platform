using Vertr.MarketData.Contracts;
using Vertr.Strategies.Contracts;

namespace Vertr.Strategies.Application.StrategiesImpl;

internal class RandomWalkStrategy : StrategyBase
{
    public RandomWalkStrategy(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override Task<TradingSignal?> CreateTradingSignal(Candle candle)
    {
        var signal = new TradingSignal
        {
            Id = Guid.NewGuid(),
            StrategyId = Id,
            InstrumentId = InstrumentId,
            BacktestId = BacktestId,
            PortfolioId = PortfolioId,
            QtyLots = QtyLots * GetSign(),
            Price = candle.Close,
            CreatedAt = candle.TimeUtc,
        };

        return Task.FromResult<TradingSignal?>(signal);
    }

    public int GetSign() => Random.Shared.Next(2) == 0 ? -1 : 1;
}
