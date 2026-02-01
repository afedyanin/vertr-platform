using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Abstractions;
using Vertr.Strategies.FuturesArbitrage.Indicators;

namespace Vertr.Strategies.FuturesArbitrage.Trading.BackgroundServices;

public class OrderBookWatcher : BackgroundService
{
    private static readonly Guid SberId = new Guid("e6123145-9665-43e0-8413-cd61b8aa9b13");

    private readonly TimeSpan _watchInterval = TimeSpan.FromSeconds(2);
    private readonly IOrderBooksLocalStorage _orderBookRepository;
    private readonly bool _isEnabled = true;
    private readonly string _serviceName;
    private readonly ILogger _logger;

    private readonly OrderBookStatsIndicator _sberIndicator;
    public OrderBookWatcher(IServiceProvider serviceProvider)
    {
        _serviceName = GetType().Name;
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        _logger = loggerFactory.CreateLogger(_serviceName);
        _orderBookRepository = serviceProvider.GetRequiredService<IOrderBooksLocalStorage>();
        _sberIndicator = new OrderBookStatsIndicator(2);
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

                lastUpdate = ExecuteWatchStep(lastUpdate);
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

    private DateTime? ExecuteWatchStep(DateTime? lastUpdate)
    {
        var book = _orderBookRepository.GetById(SberId);

        if (book == null)
        {
            _logger.LogInformation("No order books found by Id={SberId}", SberId);
            return null;
        }

        if (lastUpdate.HasValue && lastUpdate.Value == book.UpdatedAt)
        {
            // already processsd
            return book.UpdatedAt;
        }

        // создать сигнал и отправить по пайплайну

        _sberIndicator.Apply(book);
        _logger.LogInformation("Book: {Book}", book);

        if (_sberIndicator.MidPriceSignal != Common.Contracts.TradingDirection.Hold)
        {
            _logger.LogWarning("Signals: {Signals}", _sberIndicator);
        }

        return book.UpdatedAt;
    }
}
