using Vertr.Domain.Ports;

namespace Vertr.Server.BackgroundServices;

public class OrderTradesStreamService : StreamServiceBase
{
    public OrderTradesStreamService(
        ITinvestGateway tinvestGateway,
        ILogger<OrderTradesStreamService> logger) : base(tinvestGateway, logger)
    {
    }

    protected override Task Subscribe(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken stoppingToken = default)
            => TinvestGateway.SubscribeToOrderTradesStream(logger, deadline: null, stoppingToken);
}
