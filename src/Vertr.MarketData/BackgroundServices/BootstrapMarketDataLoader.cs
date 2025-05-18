
namespace Vertr.MarketData.BackgroundServices;

// При старте сервиса и периодически(?) проверяет полноту загрузки истории свечей.
// При необходимости догружает историю через API
public class BootstrapMarketDataLoader : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}
