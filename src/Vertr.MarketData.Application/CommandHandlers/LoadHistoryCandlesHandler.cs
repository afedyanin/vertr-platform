using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Commands;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.Platform.Common.Mediator;
using Vertr.Platform.Common.Utils;

namespace Vertr.MarketData.Application.CommandHandlers;

internal class LoadHistoryCandlesHandler : IRequestHandler<LoadHistoryCandlesRequest>
{
    private readonly IMarketDataGateway _marketDataGateway;
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly ICandlesHistoryRepository _candlesRepository;
    private readonly MarketDataSettings _marketDataSettings;

    private readonly ILogger<LoadIntradayCandlesHandler> _logger;

    public LoadHistoryCandlesHandler(
        IMarketDataGateway marketDataGateway,
        ISubscriptionsRepository subscriptionsRepository,
        ICandlesHistoryRepository candlesRepository,
        IOptions<MarketDataSettings> marketDataSettings,
        ILogger<LoadIntradayCandlesHandler> logger)
    {
        _logger = logger;
        _marketDataGateway = marketDataGateway;
        _subscriptionsRepository = subscriptionsRepository;
        _candlesRepository = candlesRepository;
        _marketDataSettings = marketDataSettings.Value;
    }

    public async Task Handle(LoadHistoryCandlesRequest request, CancellationToken cancellationToken)
    {
        if (_marketDataSettings.DisableBootstrapJobs)
        {
            return;
        }

        _logger.LogInformation($"{nameof(LoadIntradayCandlesHandler)} starting.");

        var subscriptions = await _subscriptionsRepository.GetAll();

        foreach (var subscription in subscriptions)
        {
            if (subscription.Disabled)
            {
                continue;
            }

            _logger.LogInformation($"Loading history candles for InstrumentId={subscription.InstrumentId}");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            for (var i = 1; i <= _marketDataSettings.MaxDaysForCandleHistory; i++)
            {
                var day = today.AddDays(-1 * i);
                var count = await LoadCandlesHistoryForDay(subscription.InstrumentId, day);
                _logger.LogInformation($"{count} candles loaded for Day={day}");
            }
        }

        _logger.LogInformation($"{nameof(LoadIntradayCandlesHandler)} completed.");
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
