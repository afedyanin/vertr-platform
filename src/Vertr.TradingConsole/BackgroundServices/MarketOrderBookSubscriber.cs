using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;
using static StackExchange.Redis.RedisChannel;

namespace Vertr.TradingConsole.BackgroundServices;

internal sealed class MarketOrderBookSubscriber : RedisServiceBase
{
    private readonly IOrderBooksLocalStorage _orderBookRepository;

    // TODO: Get from settings
    protected override RedisChannel RedisChannel => new RedisChannel("market.orderBooks", PatternMode.Pattern);
    protected override bool IsEnabled => true;

    public MarketOrderBookSubscriber(
        IServiceProvider serviceProvider,
        ILogger logger) : base(serviceProvider, logger)
    {
        _orderBookRepository = serviceProvider.GetRequiredService<IOrderBooksLocalStorage>();
    }

    public override void HandleSubscription(RedisChannel channel, RedisValue message)
    {
        var orderBook = OrderBook.FromJson(message.ToString());

        if (orderBook == null)
        {
            Logger.LogWarning("Cannot deserialize Order Book from message={Message}", message);
            return;
        }

        _orderBookRepository.Update(orderBook);

        Logger.LogDebug("Received order book from cahnnel={Channel}", channel);
    }
}
