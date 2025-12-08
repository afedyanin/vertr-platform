using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vertr.Common.Application.Services;
using Vertr.Common.Contracts;
using static StackExchange.Redis.RedisChannel;

namespace Vertr.TradingConsole.BackgroundServices;

internal sealed class PortfolioSubscriber : RedisServiceBase
{
    private readonly IPortfolioService _portfolioService;

    protected override bool IsEnabled => true;
    protected override RedisChannel RedisChannel => new RedisChannel("portfolios", PatternMode.Literal);

    public PortfolioSubscriber(IServiceProvider serviceProvider, ILogger logger) : base(serviceProvider, logger)
    {
        _portfolioService = serviceProvider.GetRequiredService<IPortfolioService>();
    }

    protected override ValueTask OnBeforeStart()
    {
        // TODO: Get All portfolios from TinvestGateway
        // TODO: Update portfolio service with protfolios

        return base.OnBeforeStart();
    }

    public override Task HandleSubscription(RedisChannel channel, RedisValue message)
    {
        Logger.LogInformation("Portfolio received: {Message}", message);

        var portfolio = Portfolio.FromJson(message.ToString());

        if (portfolio == null)
        {
            Logger.LogWarning($"Cannot deserialize portfolio.");
            return Task.CompletedTask;
        }

        _portfolioService.Update(portfolio);
        return Task.CompletedTask;
    }
}