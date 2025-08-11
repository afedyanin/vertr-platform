using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vertr.MarketData.Contracts.Commands;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.Platform.Common.Mediator;

namespace Vertr.MarketData.Application.CommandHandlers;

internal class LoadIntradayCandlesHandler : IRequestHandler<LoadIntradayCandlesRequest>
{
    private readonly IMarketDataGateway _marketDataGateway;
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly ICandlesRepository _candlesRepository;
    private readonly MarketDataSettings _marketDataSettings;


    private readonly ILogger<LoadIntradayCandlesHandler> _logger;

    public LoadIntradayCandlesHandler(
        IMarketDataGateway marketDataGateway,
        ISubscriptionsRepository subscriptionsRepository,
        ICandlesRepository candlesRepository,
        IOptions<MarketDataSettings> marketDataSettings,
        ILogger<LoadIntradayCandlesHandler> logger)
    {
        _logger = logger;
        _marketDataGateway = marketDataGateway;
        _subscriptionsRepository = subscriptionsRepository;
        _candlesRepository = candlesRepository;
        _marketDataSettings = marketDataSettings.Value;
    }

    public async Task Handle(LoadIntradayCandlesRequest request, CancellationToken cancellationToken)
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

            _logger.LogInformation($"Loading candles for InstrumentId={subscription.InstrumentId}");

            var day = DateOnly.FromDateTime(DateTime.UtcNow);
            var count = await LoadCandlesForDay(subscription.InstrumentId, day);
            _logger.LogInformation($"{count} candles loaded for Day={day}");

            day = day.AddDays(-1);
            count = await LoadCandlesForDay(subscription.InstrumentId, day);
            _logger.LogInformation($"{count} candles loaded for Day={day}");
        }

        _logger.LogInformation($"{nameof(LoadIntradayCandlesHandler)} completed.");
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
