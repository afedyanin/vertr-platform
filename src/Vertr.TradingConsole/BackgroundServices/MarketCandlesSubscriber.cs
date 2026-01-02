using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.Extensions;
using Vertr.Common.Contracts;

using static StackExchange.Redis.RedisChannel;

namespace Vertr.TradingConsole.BackgroundServices;

internal sealed class MarketCandlesSubscriber : RedisServiceBase
{
    private readonly ICandleProcessingPipeline _candleProcessingPipeline;
    private readonly IPortfolioManager _portfolioManager;
    private readonly ILogger<RedisServiceBase> _logger;

    protected override RedisChannel RedisChannel => new RedisChannel(Subscriptions.Candles.Channel, PatternMode.Pattern);
    protected override bool IsEnabled => Subscriptions.Candles.IsEnabled;

    public MarketCandlesSubscriber(IServiceProvider serviceProvider, IConfiguration configuration) : base(serviceProvider, configuration)
    {
        _candleProcessingPipeline = serviceProvider.GetRequiredService<ICandleProcessingPipeline>();
        _portfolioManager = serviceProvider.GetRequiredService<IPortfolioManager>();

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

        _candleProcessingPipeline.OnCandleEvent = (evt) =>
        {
            _logger.LogInformation(evt.Dump());
            return ValueTask.CompletedTask;
        };

        await _candleProcessingPipeline.Start(cancellationToken);
    }

    protected override async ValueTask OnBeforeStop()
    {
        await _candleProcessingPipeline.Stop();
        await _portfolioManager.CloseAllPositions();
        await base.OnBeforeStop();
    }
}