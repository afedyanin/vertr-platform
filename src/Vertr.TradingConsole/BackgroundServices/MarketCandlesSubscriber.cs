using Disruptor.Dsl;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vertr.Common.Application;
using Vertr.Common.Contracts;
using static StackExchange.Redis.RedisChannel;

namespace Vertr.TradingConsole.BackgroundServices;

internal sealed class MarketCandlesSubscriber : RedisServiceBase
{
    private readonly Disruptor<CandlestickReceivedEvent> _disruptor;

    // TODO: Get from settings
    protected override RedisChannel RedisChannel => new RedisChannel("market.candles.*", PatternMode.Pattern);
    protected override bool IsEnabled => true;

    public MarketCandlesSubscriber(
        IServiceProvider serviceProvider,
        ILogger logger) : base(serviceProvider, logger)
    {
        _disruptor = ApplicationRegistrar.CreateCandlestickPipeline(serviceProvider);
    }

    public override void HandleSubscription(RedisChannel channel, RedisValue message)
    {
        Logger.LogInformation("Received candle from cahnnel={Channel}", channel);

        var candle = Candle.FromJson(message.ToString());

        if (candle == null)
        {
            Logger.LogWarning("Cannot deserialize candle from message={Message}", message);
        }

        using (var scope = _disruptor.PublishEvent())
        {
            var evt = scope.Event();
            evt.Candle = candle;
        }
    }

    protected override ValueTask OnBeforeStart()
    {
        Logger.LogWarning("Starting Disruptor...");
        _disruptor.Start();
        return base.OnBeforeStart();
    }

    protected override ValueTask OnBeforeStop()
    {
        Logger.LogWarning("Stopping Disruptor...");
        _disruptor.Shutdown();
        return base.OnBeforeStop();
    }
}