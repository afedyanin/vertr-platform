using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.MarketData.Application.QuartzJobs;

internal static class LoadIntradayCandlesJobKeys
{
    public const string Name = "Load intraday candles job";
    public const string Group = "Market Data";

    public static readonly JobKey Key = new JobKey(Name, Group);
}

internal class LoadIntradayCandlesJob : IJob
{
    private readonly IMarketDataGateway _marketDataGateway;
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly ICandlesRepository _candlesRepository;
    private readonly MarketDataSettings _marketDataSettings;


    private readonly ILogger<LoadIntradayCandlesJob> _logger;

    public LoadIntradayCandlesJob(
        IMarketDataGateway marketDataGateway,
        ISubscriptionsRepository subscriptionsRepository,
        ICandlesRepository candlesRepository,
        IOptions<MarketDataSettings> marketDataSettings,
        ILogger<LoadIntradayCandlesJob> logger)
    {
        _logger = logger;
        _marketDataGateway = marketDataGateway;
        _subscriptionsRepository = subscriptionsRepository;
        _candlesRepository = candlesRepository;
        _marketDataSettings = marketDataSettings.Value;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"{LoadIntradayCandlesJobKeys.Name} starting.");

        var subscriptions = await _subscriptionsRepository.GetAll();

        foreach (var subscription in subscriptions)
        {
            if (subscription.Disabled)
            {
                continue;
            }

            _logger.LogInformation($"Loading candles for InstrumentId={subscription.InstrumentId}");

            var day = DateOnly.FromDateTime(DateTime.UtcNow);
            var count = await LoadCandlesForDay(subscription.InstrumentId, day);
            _logger.LogInformation($"{count} candles loaded for Day={day}");

            day = day.AddDays(-1);
            count = await LoadCandlesForDay(subscription.InstrumentId, day);
            _logger.LogInformation($"{count} candles loaded for Day={day}");
        }

        _logger.LogInformation($"{LoadIntradayCandlesJobKeys.Name} completed.");
    }

    private async Task<int> LoadCandlesForDay(Guid instrumentId, DateOnly day)
    {
        var candels = await _marketDataGateway.GetCandles(instrumentId, day, _marketDataSettings.CandleInterval);

        if (candels == null)
        {
            return 0;
        }

        var count = await _candlesRepository.Upsert(candels);
        return count;
    }
}
