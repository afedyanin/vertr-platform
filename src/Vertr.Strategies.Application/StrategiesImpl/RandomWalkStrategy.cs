using Vertr.MarketData.Contracts;
using Vertr.Strategies.Contracts;

namespace Vertr.Strategies.Application.StrategiesImpl;

internal class RandomWalkStrategy : StrategyBase
{
    public RandomWalkStrategy(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override TradingSignal CreateTradingSignal(Candle candle)
        => new TradingSignal
        {
            Id = Guid.NewGuid(),
            StrategyId = Id,
            InstrumentId = InstrumentId,
            BacktestId = BacktestId,
            SubAccountId = SubAccountId,
            QtyLots = QtyLots * GetSign(),
            CreatedAt = candle.TimeUtc,
        };

    public int GetSign() => Random.Shared.Next(2) == 0 ? -1 : 1;
}
