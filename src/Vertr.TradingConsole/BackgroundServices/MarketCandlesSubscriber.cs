using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;
using static StackExchange.Redis.RedisChannel;

namespace Vertr.TradingConsole.BackgroundServices;

internal sealed class MarketCandlesSubscriber : RedisServiceBase
{
    private readonly IPortfolioManager _portfolioManager;
    private readonly ICandlesLocalStorage _candleRepository;

    // TODO: Get from settings
    protected override RedisChannel RedisChannel => new RedisChannel("market.candles.*", PatternMode.Pattern);
    protected override bool IsEnabled => true;

    public MarketCandlesSubscriber(
        IServiceProvider serviceProvider,
        ILogger logger) : base(serviceProvider, logger)
    {
        _portfolioManager = serviceProvider.GetRequiredService<IPortfolioManager>();
        _candleRepository = serviceProvider.GetRequiredService<ICandlesLocalStorage>();
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

        // TODO: Implement this
        _candleRepository.Update(candle);
    }

    protected override async ValueTask OnBeforeStop()
    {
        // TODO: Use settings
        await _portfolioManager.CloseAllPositions();
        await base.OnBeforeStop();
    }
}