using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;
using static StackExchange.Redis.RedisChannel;

namespace Vertr.TradingConsole.BackgroundServices;

internal sealed class MarketCandlesSubscriber : RedisServiceBase
{
    private readonly ICandleProcessingPipeline _candleProcessingPipeline;

    // TODO: Get from settings
    protected override RedisChannel RedisChannel => new RedisChannel("market.candles.*", PatternMode.Pattern);
    protected override bool IsEnabled => true;

    public MarketCandlesSubscriber(
        IServiceProvider serviceProvider,
        ILogger logger) : base(serviceProvider, logger)
    {
        _candleProcessingPipeline = serviceProvider.GetRequiredService<ICandleProcessingPipeline>();
    }

    public override void HandleSubscription(RedisChannel channel, RedisValue message)
    {
        Logger.LogInformation("Received candle from cahnnel={Channel}", channel);

        var candle = Candle.FromJson(message.ToString());

        if (candle == null)
        {
            Logger.LogWarning("Cannot deserialize candle from message={Message}", message);
            return;
        }

        _candleProcessingPipeline.Handle(candle);
    }

    protected override async ValueTask OnBeforeStart(CancellationToken cancellationToken)
    {
        await base.OnBeforeStart(cancellationToken);
        await _candleProcessingPipeline.Start(verbose: true, cancellationToken);
    }

    protected override async ValueTask OnBeforeStop()
    {
        await _candleProcessingPipeline.Stop();
        await base.OnBeforeStop();
    }
}