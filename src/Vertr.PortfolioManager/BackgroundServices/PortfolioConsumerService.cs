
namespace Vertr.PortfolioManager.BackgroundServices;

public class PortfolioConsumerService : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 1. Satrt Kafka consumer for portfolio topic
        // 2. On receive Portfolio message - save snapshot to DB

        /*
        var metadata = await _metadataRepository.GetByAccountId(accountId);

        // если нет метадаты, создаем и сохраняем ее со своим portfolioId
        // если несколько метадат?

        var snapshot = portfolioResponse.Convert(portfolioId);

        if (snapshot != null)
        {
            var saved = await _snapshotRepository.Save(snapshot);
        }
        */

        throw new NotImplementedException();
    }
}
