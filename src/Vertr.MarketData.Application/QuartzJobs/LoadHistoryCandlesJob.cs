using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.Platform.Common.Utils;

namespace Vertr.MarketData.Application.QuartzJobs;

internal static class LoadHistoryCandlesJobKeys
{
    public const string Name = "Load history candles job";
    public const string Group = "Market Data";

    public static readonly JobKey Key = new JobKey(Name, Group);
}

internal class LoadHistoryCandlesJob : IJob
{
    private readonly IMarketDataGateway _marketDataGateway;
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly ICandlesHistoryRepository _candlesRepository;
    private readonly MarketDataSettings _marketDataSettings;

    private readonly ILogger<LoadIntradayCandlesJob> _logger;

    public LoadHistoryCandlesJob(
        IMarketDataGateway marketDataGateway,
        ISubscriptionsRepository subscriptionsRepository,
        ICandlesHistoryRepository candlesRepository,
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
        _logger.LogInformation($"{LoadHistoryCandlesJobKeys.Name} starting.");

        var subscriptions = await _subscriptionsRepository.GetAll();

        foreach (var subscription in subscriptions)
        {
            if (subscription.Disabled)
            {
                continue;
            }

            _logger.LogInformation($"Loading history candles for InstrumentId={subscription.InstrumentId}");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            for (int i = 1; i <= _marketDataSettings.MaxDaysForCandleHistory; i++)
            {
                var day = today.AddDays((-1) * i);
                var count = await LoadCandlesHistoryForDay(subscription.InstrumentId, day);
                _logger.LogInformation($"{count} candles loaded for Day={day}");
            }
        }

        _logger.LogInformation($"{LoadHistoryCandlesJobKeys.Name} completed.");
    }

    private async Task<int> LoadCandlesHistoryForDay(Guid instrumentId, DateOnly day)
    {
        var candles = await _marketDataGateway.GetCandles(instrumentId, day, _marketDataSettings.CandleInterval);

        if (candles == null)
        {
            return 0;
        }

        var item = new CandlesHistoryItem
        {
            Id = Guid.NewGuid(),
            InstrumentId = instrumentId,
            Day = day,
            Interval = _marketDataSettings.CandleInterval,
            Count = candles.Length,
            Data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(candles, JsonOptions.DefaultOptions)),
        };

        var saved = await _candlesRepository.Save(item);

        if (!saved)
        {
            _logger.LogError($"Cannot save candles history for day={day}.");
            return 0;
        }

        return candles.Length;
    }
}
