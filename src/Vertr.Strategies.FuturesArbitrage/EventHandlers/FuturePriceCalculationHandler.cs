using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Abstractions;

namespace Vertr.Strategies.FuturesArbitrage.EventHandlers;

internal sealed class FuturePriceCalculationHandler : IEventHandler<OrderBookChangedEvent>
{
    private readonly ILogger<FuturePriceCalculationHandler> _logger;

    public int HandlingOrder => 20;

    public FuturePriceCalculationHandler(ILogger<FuturePriceCalculationHandler> logger)
    {
        _logger = logger;
    }

    public ValueTask OnEvent(OrderBookChangedEvent data)
    {
        _logger.LogDebug("Processing event #{Sequence}. Futures=[{Futures}]", data.Sequence, string.Join(',', data.FairPrices.Keys));

        /*
        if (data.TradingDirection == Common.Contracts.TradingDirection.Hold)
        {
            return ValueTask.CompletedTask;
        }

        foreach (var instrumentId in data.FairPrices.Keys)
        {
            Debug.Assert(instrumentId != Guid.Empty);
            // получить ExpDate из статик даты (MOEX)
            // получить спот цену базового актива из стакана -> data.OrderBook
            // получить рейт RUSFAR в соответствии с ExpDate (MOEX)
            // посчитать цену и положить в словарь
        }
        */

        return ValueTask.CompletedTask;
    }
}
