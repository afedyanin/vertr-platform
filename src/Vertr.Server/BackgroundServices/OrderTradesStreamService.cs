using Grpc.Core;
using Vertr.Domain.Ports;

namespace Vertr.Server.BackgroundServices;

public class OrderTradesStreamService : BackgroundService
{
    private readonly ITinvestGateway _tinvestGateway;
    private readonly ILogger<OrderTradesStreamService> _logger;

    public OrderTradesStreamService(
        ITinvestGateway tinvestGateway,
        ILogger<OrderTradesStreamService> logger)
    {
        _tinvestGateway = tinvestGateway;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await StartConsumingLoop(stoppingToken);
            _logger.LogInformation($"{nameof(OrderTradesStreamService)} execution completed at {DateTime.UtcNow:O}");
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
                _logger.LogInformation($"{nameof(OrderTradesStreamService)} stream started at {DateTime.UtcNow:O}");
                await _tinvestGateway.SubscribeToOrderTradesStream(_logger, deadline: null, stoppingToken);
            }
            catch (RpcException rpcEx)
            {
                if (rpcEx.StatusCode != StatusCode.DeadlineExceeded)
                {
                    _logger.LogError(rpcEx, $"{nameof(OrderTradesStreamService)} consuming exception. Message={rpcEx.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(OrderTradesStreamService)} consuming exception. Message={ex.Message}");
            }
        }
    }
}
