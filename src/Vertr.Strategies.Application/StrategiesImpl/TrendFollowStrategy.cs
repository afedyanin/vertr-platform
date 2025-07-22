using Vertr.MarketData.Contracts;
using Vertr.OrderExecution.Contracts;

namespace Vertr.Strategies.Application.StrategiesImpl;

internal class TrendFollowStrategy : StrategyBase
{
    public TrendFollowStrategy(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override TradingSignal CreateTradingSignal(Candle candle)
        => new TradingSignal
        {
            RequestId = Guid.NewGuid(),
            InstrumentId = InstrumentId,
            PortfolioIdentity = PortfolioIdentity,
            QtyLots = QtyLots * GetSign(candle),
        };

    private int GetSign(Candle candle) => candle.Open > candle.Close ? -1 : 1;
}
