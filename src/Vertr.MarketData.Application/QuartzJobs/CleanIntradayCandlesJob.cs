using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.MarketData.Application.QuartzJobs;

internal static class CleanIntradayCandlesJobKeys
{
    public const string Name = "Clean intraday candles job";
    public const string Group = "Market Data";

    public static readonly JobKey Key = new JobKey(Name, Group);
}

internal class CleanIntradayCandlesJob : IJob
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly ICandlesRepository _candlesRepository;
    private readonly MarketDataSettings _marketDataSettings;

    private readonly ILogger<CleanIntradayCandlesJob> _logger;

    public CleanIntradayCandlesJob(
        ISubscriptionsRepository subscriptionsRepository,
        ICandlesRepository candlesRepository,
        IOptions<MarketDataSettings> marketDataSettings,
        ILogger<CleanIntradayCandlesJob> logger)
    {
        _logger = logger;
        _subscriptionsRepository = subscriptionsRepository;
        _candlesRepository = candlesRepository;
        _marketDataSettings = marketDataSettings.Value;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"{CleanIntradayCandlesJobKeys.Name} starting.");

        var subscriptions = await _subscriptionsRepository.GetAll();

        var today = DateTime.UtcNow.Date;
        var dayBefore = today.AddDays(-1 * _marketDataSettings.RemoveIntradayCandlesBeforeDays);

        foreach (var subscription in subscriptions)
        {
            if (subscription.Disabled)
            {
                //continue;
            }

            _logger.LogInformation($"Loading candles for InstrumentId={subscription.InstrumentId}");

            var removed = await _candlesRepository.Delete(subscription.InstrumentId, dayBefore);
            _logger.LogInformation($"{removed} candles deleted for InstrumentId={subscription.InstrumentId}");
        }

        _logger.LogInformation($"{CleanIntradayCandlesJobKeys.Name} completed.");
    }
}
