using Grpc.Core;
using Vertr.Domain.Ports;

namespace Vertr.Server.BackgroundServices;

public abstract class StreamServiceBase : BackgroundService
{
    protected ITinvestGateway TinvestGateway { get; private set; }

    protected ILogger Logger { get; private set; }

    private readonly string _serviceName;

    protected StreamServiceBase(
        ITinvestGateway tinvestGateway,
        ILogger logger)
    {
        TinvestGateway = tinvestGateway;
        Logger = logger;

        _serviceName = GetType().Name;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await StartConsumingLoop(stoppingToken);
            Logger.LogInformation($"{_serviceName} execution completed at {DateTime.UtcNow:O}");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }
    }

    private async Task StartConsumingLoop(CancellationToken stoppingToken)
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
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"{_serviceName} consuming exception. Message={ex.Message}");
            }
        }
    }

    protected abstract Task Subscribe(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken stoppingToken = default);
}
