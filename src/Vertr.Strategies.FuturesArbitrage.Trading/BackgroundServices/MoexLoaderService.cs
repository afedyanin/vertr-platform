using Microsoft.Extensions.Hosting;

namespace Vertr.Strategies.FuturesArbitrage.Trading.BackgroundServices;

internal sealed class MoexLoaderService : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}
