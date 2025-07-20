using Vertr.MarketData.Contracts;
using Vertr.OrderExecution.Contracts.Requests;

namespace Vertr.Strategies.Application.StrategiesImpl;

internal class RandomWalkStrategy : StrategyBase
{
    public RandomWalkStrategy(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override TradingSignalRequest CreateSignal(Candle candle)
        => new TradingSignalRequest
        {
            RequestId = Guid.NewGuid(),
            InstrumentId = InstrumentId,
            PortfolioIdentity = PortfolioIdentity,
            QtyLots = QtyLots * GetSign(),
        };

    public int GetSign()
        => Random.Shared.Next(2) == 0 ? -1 : 1;
}
