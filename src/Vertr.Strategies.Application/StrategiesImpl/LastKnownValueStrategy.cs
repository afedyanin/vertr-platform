using Microsoft.Extensions.DependencyInjection;
using Vertr.MarketData.Contracts;
using Vertr.Strategies.Contracts;
using Vertr.Strategies.Contracts.Interfaces;

namespace Vertr.Strategies.Application.StrategiesImpl;

internal class LastKnownValueStrategy : StrategyBase
{
    private readonly IPredictionService _predictionService;

    public LastKnownValueStrategy(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _predictionService = ServiceProvider.GetRequiredService<IPredictionService>();
    }

    public override async Task<TradingSignal?> CreateTradingSignal(Candle candle)
    {
        var res = await _predictionService.Predict(StrategyType.LastKnownValue, [candle]);
        var nextPrice = res.GetDecimal("next");

        if (nextPrice == null)
        {
            return null;
        }

        var signal = new TradingSignal
        {
            Id = Guid.NewGuid(),
            StrategyId = Id,
            InstrumentId = InstrumentId,
            BacktestId = BacktestId,
            PortfolioId = PortfolioId,
            QtyLots = QtyLots * GetSign(candle, nextPrice.Value),
            Price = candle.Close,
            CreatedAt = candle.TimeUtc,
        };

        return signal;
    }

    private int GetSign(Candle candle, decimal nextPrice) => nextPrice > candle.Close ? 1 : -1;
}
