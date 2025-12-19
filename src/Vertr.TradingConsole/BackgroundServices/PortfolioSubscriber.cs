using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;
using static StackExchange.Redis.RedisChannel;

namespace Vertr.TradingConsole.BackgroundServices;

internal sealed class PortfolioSubscriber : RedisServiceBase
{
    private readonly IPortfoliosLocalStorage _portfolioRepository;

    protected override bool IsEnabled => true;
    protected override RedisChannel RedisChannel => new RedisChannel("portfolios", PatternMode.Literal);

    public PortfolioSubscriber(IServiceProvider serviceProvider, ILogger logger) : base(serviceProvider, logger)
    {
        _portfolioRepository = serviceProvider.GetRequiredService<IPortfoliosLocalStorage>();
    }

    protected override ValueTask OnBeforeStart(CancellationToken cancellationToken)
    {
        // TODO: Get predictors from config or args
        _portfolioRepository.Init(["RandomWalk"]);
        return base.OnBeforeStart(cancellationToken);
    }

    public override void HandleSubscription(RedisChannel channel, RedisValue message)
    {
        Logger.LogInformation("Portfolio received: {Message}", message);

        var portfolio = Portfolio.FromJson(message.ToString());

        if (portfolio == null)
        {
            Logger.LogWarning($"Cannot deserialize portfolio.");
            return;
        }

        _portfolioRepository.Update(portfolio);
    }
}