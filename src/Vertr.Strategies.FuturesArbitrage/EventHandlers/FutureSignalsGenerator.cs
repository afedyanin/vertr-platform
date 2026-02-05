using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Abstractions;

namespace Vertr.Strategies.FuturesArbitrage.EventHandlers;

internal sealed class FutureSignalsGenerator : IEventHandler<OrderBookChangedEvent>
{
    private readonly IOrderBooksLocalStorage _orderBooksLocalStorage;
    private readonly ILogger<FutureSignalsGenerator> _logger;

    public int HandlingOrder => 30;

    public FutureSignalsGenerator(
        IOrderBooksLocalStorage orderBooksLocalStorage,
        ILogger<FutureSignalsGenerator> logger)
    {
        _orderBooksLocalStorage = orderBooksLocalStorage;
        _logger = logger;
    }

    public ValueTask OnEvent(OrderBookChangedEvent data)
    {
        _logger.LogDebug("Processing event #{Sequence}", data.Sequence);

        /*
        if (data.TradingDirection == Common.Contracts.TradingDirection.Hold)
        {
            return ValueTask.CompletedTask;
        }

        foreach (var kvp in data.FairPrices)
        {
            if (!kvp.Value.HasValue)
            {
                continue;
            }

            var orderBook = _orderBooksLocalStorage.GetById(kvp.Key);

            if (orderBook == null)
            {
                continue;
            }

            // var fairPrice = kvp.Value.Value;
            // сравнить словарь fairPrice с Bid,Ask квотами в стаканах по фьючам
            // проверить на значимость отклонений по threshold-ам
            // сгенерить сигналы и положить в коллекцию TradingSignals
        }
        */

        return ValueTask.CompletedTask;
    }
}
