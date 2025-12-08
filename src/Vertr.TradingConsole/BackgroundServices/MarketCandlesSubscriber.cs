using Disruptor.Dsl;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vertr.Common.Application;
using Vertr.Common.Contracts;
using static StackExchange.Redis.RedisChannel;

namespace Vertr.TradingConsole.BackgroundServices;

internal sealed class MarketCandlesSubscriber : RedisServiceBase
{
    private readonly Dictionary<string, Guid> _redisChannels = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
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

    public override async Task HandleSubscription(RedisChannel channel, RedisValue message)
    {
        Logger.LogInformation($"Received candlestick: {channel} - {message}");

        var candlestick = Candlestick.FromJson(message.ToString());

        if (candlestick == null)
        {
            Logger.LogWarning("Cannot deserialize candlestick from message={Message}", message);
            return;
        }

        var instrumentId = GetInstrumentId(channel);

        if (instrumentId == null)
        {
            Logger.LogWarning("Cannot determine InstrumentId from channel={Channel}", channel.ToString());
            return;
        }

        using (var scope = _disruptor.PublishEvent())
        {
            var evt = scope.Event();
            evt.Candle = Candle.FromCandlestick(candlestick.Value, instrumentId.Value);
        }
    }

    private Guid? GetInstrumentId(RedisChannel channel)
    {
        if (_redisChannels.TryGetValue(channel.ToString(), out var instrumentId))
        {
            return instrumentId;
        }

        // TODO: Implement this
        return Guid.NewGuid();
    }
}