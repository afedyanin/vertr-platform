
namespace Vertr.PortfolioManager.BackgroundServices;

public class PortfolioConsumerService : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 1. Satrt Kafka consumer for portfolio topic
        // 2. On receive Portfolio message - save snapshot to DB

        /*
        var snapshot = portfolioResponse.Convert();

        if (snapshot != null)
        {
            var saved = await _snapshotRepository.Save(snapshot);
        }
        */

        throw new NotImplementedException();
    }
}
