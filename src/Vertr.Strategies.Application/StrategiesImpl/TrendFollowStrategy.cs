using Vertr.MarketData.Contracts;
using Vertr.OrderExecution.Contracts.Commands;

namespace Vertr.Strategies.Application.StrategiesImpl;

internal class TrendFollowStrategy : StrategyBase
{
    public TrendFollowStrategy(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override TradingSignalCommand CreateSignal(Candle candle)
    {
        var signal = new TradingSignalCommand
        {
            RequestId = Guid.NewGuid(),
            InstrumentId = InstrumentId,
            PortfolioIdentity = PortfolioIdentity,
            QtyLots = QtyLots * GetSign(candle),
        };

        return signal;
    }

    private int GetSign(Candle candle)
        => candle.Open > candle.Close ? -1 : 1;
}
