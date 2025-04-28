using Vertr.Domain.Ports;

namespace Vertr.Server.BackgroundServices;

public class MarketDataStreamService : StreamServiceBase
{
    public MarketDataStreamService(
        ITinvestGateway tinvestGateway,
        ILogger<MarketDataStreamService> logger) : base(tinvestGateway, logger)
    {
    }

    protected override Task Subscribe(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken stoppingToken = default)
            => TinvestGateway.SubscribeToMarketDataStream(logger, deadline: null, stoppingToken);
}
