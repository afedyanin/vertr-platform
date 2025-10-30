using Microsoft.AspNetCore.Http.HttpResults;
using Vertr.MarketData.Contracts;
using Vertr.Strategies.Contracts;

namespace Vertr.Strategies.Application.StrategiesImpl;

internal class TrendFollowStrategy : StrategyBase
{
    public TrendFollowStrategy(IServiceProvider serviceProvider) : base(serviceProvider)
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
            QtyLots = QtyLots * GetSign(candle),
            Price = candle.Close,
            CreatedAt = candle.TimeUtc,
        };

        return Task.FromResult<TradingSignal?>(signal);
    }

    private int GetSign(Candle candle) => candle.Open > candle.Close ? -1 : 1;
}
