using Vertr.Domain.Ports;

namespace Vertr.Server.BackgroundServices;

public class PortfolioStreamService : StreamServiceBase
{
    public PortfolioStreamService(
        ITinvestGateway tinvestGateway,
        ILogger<PortfolioStreamService> logger) : base(tinvestGateway, logger)
    {
    }

    protected override Task Subscribe(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken stoppingToken = default)
            => TinvestGateway.SubscribeToPortfolioStream(logger, deadline: null, stoppingToken);
}
