using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Vertr.TradingConsole.BackgroundServices;

internal abstract class RedisServiceBase : BackgroundService
{
    protected IServiceProvider ServiceProvider { get; private set; }
    protected IConnectionMultiplexer Redis { get; private set; }

    protected ILoggerFactory LoggerFactory { get; private set; }

    protected abstract RedisChannel RedisChannel { get; }

    protected abstract bool IsEnabled { get; }

    private readonly string _serviceName;

    private ISubscriber? _subscriber;

    private readonly ILogger<RedisServiceBase> _logger;

    protected RedisServiceBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        LoggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        Redis = serviceProvider.GetRequiredService<IConnectionMultiplexer>();
        _serviceName = GetType().Name;
        _logger = LoggerFactory.CreateLogger<RedisServiceBase>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            if (!IsEnabled)
            {
                _logger.LogWarning($"{_serviceName} is disabled.");
                return;
            }

            await OnBeforeStart(stoppingToken);
            _subscriber = Redis.GetSubscriber();
            await _subscriber.SubscribeAsync(RedisChannel, (channel, message) =>
            {
                try
                {
                    HandleSubscription(channel, message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "HandleSubscription error. Message={Message}", ex.Message);
                }
            });
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        _logger.LogInformation($"{_serviceName} execution completed at {DateTime.UtcNow:O}");
    }

    public abstract void HandleSubscription(RedisChannel channel, RedisValue message);

    protected virtual ValueTask OnBeforeStart(CancellationToken cancellationToken)
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