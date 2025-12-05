using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using static StackExchange.Redis.RedisChannel;

namespace Vertr.TradingConsole.BackgroundServices;

internal sealed class PortfolioSubscriber : RedisServiceBase
{
    protected override bool IsEnabled => true;

    protected override RedisChannel Channel => new RedisChannel("portfolios", PatternMode.Literal);

    public PortfolioSubscriber(IServiceProvider serviceProvider, ILogger logger) : base(serviceProvider, logger)
    {
    }

    public override Task HandleSubscription(RedisChannel channel, RedisValue message)
    {
        // var portfolio = Portfolio.FromJson(message.ToString());
        Logger.LogInformation($"Portfolio received: {message}");
        return Task.CompletedTask;
    }
}