using Vertr.MarketData.Contracts;
using Vertr.OrderExecution.Contracts;

namespace Vertr.Strategies.Application.StrategiesImpl;

internal class RandomWalkStrategy : StrategyBase
{
    public RandomWalkStrategy(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override TradingSignal CreateTradingSignal(Candle candle)
        => new TradingSignal
        {
            RequestId = Guid.NewGuid(),
            InstrumentId = InstrumentId,
            PortfolioIdentity = PortfolioIdentity,
            QtyLots = QtyLots * GetSign(),
        };

    public int GetSign() => Random.Shared.Next(2) == 0 ? -1 : 1;
}
