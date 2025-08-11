using Vertr.Platform.Common.Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vertr.MarketData.Contracts.Commands;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.MarketData.Application.CommandHandlers;

internal class CleanIntradayCandlesHandler : IRequestHandler<CleanIntradayCandlesRequest>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly ICandlesRepository _candlesRepository;
    private readonly MarketDataSettings _marketDataSettings;

    private readonly ILogger<CleanIntradayCandlesHandler> _logger;

    public CleanIntradayCandlesHandler(
        ISubscriptionsRepository subscriptionsRepository,
        ICandlesRepository candlesRepository,
        IOptions<MarketDataSettings> marketDataSettings,
        ILogger<CleanIntradayCandlesHandler> logger)
    {
        _logger = logger;
        _subscriptionsRepository = subscriptionsRepository;
        _candlesRepository = candlesRepository;
        _marketDataSettings = marketDataSettings.Value;
    }

    public async Task Handle(CleanIntradayCandlesRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(CleanIntradayCandlesHandler)} starting.");

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

        _logger.LogInformation($"{nameof(CleanIntradayCandlesHandler)} completed.");
    }
}
