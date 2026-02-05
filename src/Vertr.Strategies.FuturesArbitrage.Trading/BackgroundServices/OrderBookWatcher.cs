using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.Configuration;
using Vertr.Strategies.FuturesArbitrage.Abstractions;

namespace Vertr.Strategies.FuturesArbitrage.Trading.BackgroundServices;

public class OrderBookWatcher : BackgroundService
{
    private readonly TimeSpan _watchInterval = TimeSpan.FromSeconds(2);
    private readonly IFuturesProcessingPipeline _futuresProcessingPipeline;
    private readonly IOrderBooksLocalStorage _orderBookRepository;
    private readonly bool _isEnabled = true;
    private readonly string _serviceName;
    private readonly ILogger _logger;
    private readonly InstrumentSettings _instrumentSettings;

    private int _eventSequence;

    public OrderBookWatcher(IServiceProvider serviceProvider)
    {
        _serviceName = GetType().Name;
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        _logger = loggerFactory.CreateLogger(_serviceName);
        _futuresProcessingPipeline = serviceProvider.GetRequiredService<IFuturesProcessingPipeline>();
        _orderBookRepository = serviceProvider.GetRequiredService<IOrderBooksLocalStorage>();
        _instrumentSettings = serviceProvider.GetRequiredService<IOptions<InstrumentSettings>>().Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        DateTime? lastUpdate = null;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!_isEnabled)
                {
                    _logger.LogWarning($"{_serviceName} is disabled.");
                    return;
                }

                lastUpdate = await ExecuteWatchStep(lastUpdate);
                await Task.Delay(_watchInterval, stoppingToken);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex.Message);
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        _logger.LogInformation($"{_serviceName} execution completed at {DateTime.UtcNow:O}");
    }

    private async Task<DateTime?> ExecuteWatchStep(DateTime? lastUpdate)
    {
        var basicAssetId = _instrumentSettings.BasicAsset;
        var book = _orderBookRepository.GetById(basicAssetId);

        if (book == null)
        {
            _logger.LogWarning("No order books found by Id={BasicAsset}", basicAssetId);
            return null;
        }

        if (lastUpdate.HasValue && lastUpdate.Value == book.UpdatedAt)
        {
            // already processsd
            return book.UpdatedAt;
        }

        var derivedAssetPrices = _instrumentSettings.DerivedAssets.ToDictionary(item => item, _ => (decimal?)null);

        var evt = new OrderBookChangedEvent
        {
            Sequence = _eventSequence++,
            OrderBook = book,
            FairPrices = derivedAssetPrices,
        };

        _logger.LogDebug("#{Sequence} Book={Book}", evt.Sequence, evt.OrderBook);
        await _futuresProcessingPipeline.Handle(evt);

        return book.UpdatedAt;
    }
}
