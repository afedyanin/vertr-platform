using Microsoft.Extensions.Configuration;
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
    private readonly ILogger<MarketOrderBookSubscriber> _logger;

    protected override RedisChannel RedisChannel => new RedisChannel(Subscriptions.OrderBooks.Channel, PatternMode.Pattern);
    protected override bool IsEnabled => Subscriptions.OrderBooks.IsEnabled;

    public MarketOrderBookSubscriber(IServiceProvider serviceProvider, IConfiguration configuration) : base(serviceProvider, configuration)
    {
        _orderBookRepository = serviceProvider.GetRequiredService<IOrderBooksLocalStorage>();
        _logger = LoggerFactory.CreateLogger<MarketOrderBookSubscriber>();
    }

    public override void HandleSubscription(RedisChannel channel, RedisValue message)
    {
        var orderBook = OrderBook.FromJson(message.ToString());

        if (orderBook == null)
        {
            _logger.LogWarning("Cannot deserialize Order Book from message={Message}", message);
            return;
        }

        _orderBookRepository.Update(orderBook);
        _logger.LogDebug("Received order book from cahnnel={Channel}", channel);
    }
}
