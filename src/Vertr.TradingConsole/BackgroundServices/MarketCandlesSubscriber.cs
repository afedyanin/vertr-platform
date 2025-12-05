using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts;
using Vertr.TinvestGateway.Contracts.MarketData;
using Vertr.TinvestGateway.Contracts.Orders;
using Vertr.TinvestGateway.Contracts.Orders.Enums;
using static StackExchange.Redis.RedisChannel;

namespace Vertr.TradingConsole.BackgroundServices;

internal sealed class MarketCandlesSubscriber : RedisServiceBase
{
    private readonly ITinvestGatewayClient _gatewayClient;

    private static readonly Guid InstrumentId = new("e6123145-9665-43e0-8413-cd61b8aa9b13");
    private static readonly Guid PortfolioId = new("D4713DA2-ED51-4AE2-A31E-3CB986649796");

    protected override bool IsEnabled => true;

    protected override RedisChannel Channel => new RedisChannel("market.candles*", PatternMode.Pattern);

    public MarketCandlesSubscriber(
        IServiceProvider serviceProvider,
        ILogger logger) : base(serviceProvider, logger)
    {
        _gatewayClient = serviceProvider.GetRequiredService<ITinvestGatewayClient>();
    }

    public override async Task HandleSubscription(RedisChannel channel, RedisValue message)
    {
        var candle = Candlestick.FromJson(message.ToString());
        Console.WriteLine($"Received candle: {channel} - {candle}");
        await PostRandomOrder();
    }
    private async Task PostRandomOrder()
    {
        var request = new PostOrderRequest
        {
            RequestId = Guid.NewGuid(),
            InstrumentId = InstrumentId, // Get from channel name
            PortfolioId = PortfolioId, // Get From Strategy Dictionary
            OrderDirection = GetRandomDirection(),
            OrderType = OrderType.Market,
            TimeInForceType = TimeInForceType.Unspecified,
            PriceType = PriceType.Unspecified,
            Price = 0.0m,
            QuantityLots = 10,
            CreatedAt = DateTime.UtcNow,
        };

        if (_gatewayClient != null)
        {
            var response = await _gatewayClient.PostOrder(request);
            Console.WriteLine($"Post order response: {response}");
        }
    }

    private static OrderDirection GetRandomDirection()
        => Random.Shared.Next(0, 2) == 0 ? OrderDirection.Buy : OrderDirection.Sell;
}