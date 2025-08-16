using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vertr.MarketData.Contracts.Commands;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.Platform.Common.Mediator;

namespace Vertr.MarketData.Application.CommandHandlers;

internal class LoadHistoryCandlesHandler : IRequestHandler<LoadHistoryCandlesRequest>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly MarketDataSettings _marketDataSettings;
    private readonly ICandlesHistoryLoader _candlesHistoryLoader;

    private readonly ILogger<LoadIntradayCandlesHandler> _logger;

    public LoadHistoryCandlesHandler(
        ICandlesHistoryLoader candlesHistoryLoader,
        ISubscriptionsRepository subscriptionsRepository,
        IOptions<MarketDataSettings> marketDataSettings,
        ILogger<LoadIntradayCandlesHandler> logger)
    {
        _logger = logger;
        _candlesHistoryLoader = candlesHistoryLoader;
        _subscriptionsRepository = subscriptionsRepository;
        _marketDataSettings = marketDataSettings.Value;
    }

    public async Task Handle(LoadHistoryCandlesRequest request, CancellationToken cancellationToken)
    {
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
                var item = await _candlesHistoryLoader.GetCandlesHistory(
                    subscription.InstrumentId,
                    day,
                    force: true);

                if (item != null)
                {
                    _logger.LogInformation($"{item.Count} candles loaded for Day={day}");
                }
            }
        }

        _logger.LogInformation($"{nameof(LoadIntradayCandlesHandler)} completed.");
    }
}
