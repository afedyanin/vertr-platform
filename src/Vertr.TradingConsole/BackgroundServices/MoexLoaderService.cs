using Microsoft.Extensions.Hosting;

namespace Vertr.TradingConsole.BackgroundServices;

internal class MoexLoaderService : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}
