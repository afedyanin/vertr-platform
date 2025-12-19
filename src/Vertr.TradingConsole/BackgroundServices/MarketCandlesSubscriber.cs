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
    private readonly ILogger<RedisServiceBase> _logger;

    // TODO: Get from settings
    protected override RedisChannel RedisChannel => new RedisChannel("market.candles.*", PatternMode.Pattern);
    protected override bool IsEnabled => true;


    public MarketCandlesSubscriber(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _candleProcessingPipeline = serviceProvider.GetRequiredService<ICandleProcessingPipeline>();
        _logger = LoggerFactory.CreateLogger<MarketCandlesSubscriber>();
    }

    public override void HandleSubscription(RedisChannel channel, RedisValue message)
    {
        _logger.LogDebug("Received candle from cahnnel={Channel}", channel);

        var candle = Candle.FromJson(message.ToString());

        if (candle == null)
        {
            _logger.LogWarning("Cannot deserialize candle from message={Message}", message);
            return;
        }

        _candleProcessingPipeline.Handle(candle);
    }

    protected override async ValueTask OnBeforeStart(CancellationToken cancellationToken)
    {
        await base.OnBeforeStart(cancellationToken);

        // dumpPortfolios = false due to order exec delay
        await _candleProcessingPipeline.Start(dumpPortfolios: false, cancellationToken);
    }

    protected override async ValueTask OnBeforeStop()
    {
        await _candleProcessingPipeline.Stop();
        await base.OnBeforeStop();
    }
}