using Vertr.MarketData.Contracts;
using Vertr.Strategies.Contracts;

namespace Vertr.Strategies.Application.StrategiesImpl;

internal class TrendFollowStrategy : StrategyBase
{
    public TrendFollowStrategy(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override TradingSignal CreateTradingSignal(Candle candle)
        => new TradingSignal
        {
            Id = Guid.NewGuid(),
            StrategyId = Id,
            InstrumentId = InstrumentId,
            AccountId = AccountId,
            SubAccountId = SubAccountId,
            QtyLots = QtyLots * GetSign(candle),
            CreatedAt = DateTime.UtcNow,
        };

    private int GetSign(Candle candle) => candle.Open > candle.Close ? -1 : 1;
}
