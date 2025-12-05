using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Vertr.TradingConsole.BackgroundServices;

internal abstract class RedisServiceBase : BackgroundService
{
    protected IServiceProvider ServiceProvider { get; private set; }
    protected IConnectionMultiplexer Redis { get; private set; }

    protected abstract RedisChannel Channel { get; }

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

            _subscriber = Redis.GetSubscriber();
            await _subscriber.SubscribeAsync(Channel, (channel, message) => HandleSubscription(channel, message));
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }

        Logger.LogInformation($"{_serviceName} execution completed at {DateTime.UtcNow:O}");
    }

    public abstract Task HandleSubscription(RedisChannel channel, RedisValue message);

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_subscriber != null)
        {
            await _subscriber.UnsubscribeAsync(Channel);
        }

        await base.StopAsync(cancellationToken);
    }
}