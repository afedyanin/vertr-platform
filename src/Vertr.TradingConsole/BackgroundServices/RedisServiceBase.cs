using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Vertr.TradingConsole.BackgroundServices;

internal abstract class RedisServiceBase : BackgroundService
{
    protected IServiceProvider ServiceProvider { get; private set; }
    protected IConnectionMultiplexer Redis { get; private set; }

    protected abstract RedisChannel RedisChannel { get; }

    protected abstract bool IsEnabled { get; }

    protected ILogger Logger { get; private set; }

    private readonly string _serviceName;

    private ISubscriber? _subscriber;

    protected RedisServiceBase(
        IServiceProvider serviceProvider,
        ILogger logger)
    {
        ServiceProvider = serviceProvider;
        Logger = logger;
        Redis = serviceProvider.GetRequiredService<IConnectionMultiplexer>();

        _serviceName = GetType().Name;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            if (!IsEnabled)
            {
                Logger.LogWarning($"{_serviceName} is disabled.");
                return;
            }

            await OnBeforeStart();
            _subscriber = Redis.GetSubscriber();
            await _subscriber.SubscribeAsync(RedisChannel, (channel, message) =>
            {
                try
                {
                    HandleSubscription(channel, message);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "HandleSubscription error. Message={Message}", ex.Message);
                }
            });
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }

        Logger.LogInformation($"{_serviceName} execution completed at {DateTime.UtcNow:O}");
    }

    public abstract void HandleSubscription(RedisChannel channel, RedisValue message);

    protected virtual ValueTask OnBeforeStart()
    {
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask OnBeforeStop()
    {
        return ValueTask.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await OnBeforeStop();

        if (_subscriber != null)
        {
            await _subscriber.UnsubscribeAsync(RedisChannel);
        }

        await base.StopAsync(cancellationToken);
    }
}