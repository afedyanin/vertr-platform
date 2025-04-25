using Grpc.Core;
using Vertr.Domain.Ports;

namespace Vertr.Server.BackgroundServices;

public class OrderStateStreamService : BackgroundService
{
    private readonly ITinvestGateway _tinvestGateway;
    private readonly ILogger<OrderStateStreamService> _logger;

    public OrderStateStreamService(
        ITinvestGateway tinvestGateway,
        ILogger<OrderStateStreamService> logger)
    {
        _tinvestGateway = tinvestGateway;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await StartConsumingLoop(stoppingToken);
            _logger.LogInformation($"{nameof(OrderStateStreamService)} execution completed at {DateTime.UtcNow:O}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    private async Task StartConsumingLoop(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation($"{nameof(OrderStateStreamService)} stream started at {DateTime.UtcNow:O}");
                await _tinvestGateway.SubscribeToOrderStateStream(_logger, deadline: null, stoppingToken);
            }
            catch (RpcException rpcEx)
            {
                if (rpcEx.StatusCode != StatusCode.DeadlineExceeded)
                {
                    _logger.LogError(rpcEx, $"{nameof(OrderStateStreamService)} consuming exception. Message={rpcEx.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(OrderStateStreamService)} consuming exception. Message={ex.Message}");
            }
        }
    }
}
