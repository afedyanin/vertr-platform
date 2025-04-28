using Vertr.Domain.Ports;

namespace Vertr.Server.BackgroundServices;

public class PositionsStreamService : StreamServiceBase
{
    public PositionsStreamService(
        ITinvestGateway tinvestGateway,
        ILogger<PositionsStreamService> logger) : base(tinvestGateway, logger)
    {
    }

    protected override Task Subscribe(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken stoppingToken = default)
            => TinvestGateway.SubscribeToPositionsStream(logger, deadline: null, stoppingToken);
}
