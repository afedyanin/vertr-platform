using Grpc.Core;
using Microsoft.Extensions.Options;

namespace Vertr.TinvestGateway.Host.BackgroundServices;

public abstract class StreamServiceBase : BackgroundService
{
    protected IServiceProvider ServiceProvider { get; private set; }

    protected TinvestSettings TinvestSettings { get; private set; }

    protected abstract bool IsEnabled { get; }

    protected ILogger Logger { get; private set; }

    private readonly string _serviceName;

    protected StreamServiceBase(
        IServiceProvider serviceProvider,
        IOptions<TinvestSettings> tinvestOptions,
        ILogger logger)
    {
        ServiceProvider = serviceProvider;
        TinvestSettings = tinvestOptions.Value;
        Logger = logger;

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

            await OnBeforeStart(stoppingToken);

            await StartConsumingLoop(stoppingToken);

            Logger.LogInformation($"{_serviceName} execution completed at {DateTime.UtcNow:O}");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }
    }

    protected virtual Task OnBeforeStart(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    protected virtual async Task StartConsumingLoop(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Logger.LogInformation($"{_serviceName} started at {DateTime.UtcNow:O}");
                await Subscribe(Logger, deadline: null, stoppingToken);
            }
            catch (RpcException rpcEx)
            {
                if (rpcEx.StatusCode != StatusCode.DeadlineExceeded)
                {
                    Logger.LogError(rpcEx, $"{_serviceName} consuming exception. Message={rpcEx.Message}");
                }

                await Task.Delay(5000, stoppingToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"{_serviceName} consuming exception. Message={ex.Message}");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }

    protected abstract Task Subscribe(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken stoppingToken = default);
}