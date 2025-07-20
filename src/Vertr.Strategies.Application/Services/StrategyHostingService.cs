using Microsoft.Extensions.Hosting;

namespace Vertr.Strategies.Application.Services;

internal class StrategyHostingService : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Load strategies metadata from DB
        // Create strategies by Factory
        // Start consuming market data
        // Rote received market data to each strategy instance

        return Task.CompletedTask;
    }
}
