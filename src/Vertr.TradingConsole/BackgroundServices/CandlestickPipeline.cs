using Disruptor.Dsl;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application;

namespace Vertr.TradingConsole.BackgroundServices;

internal class CandlestickPipeline : BackgroundService
{
    private readonly Disruptor<CandlestickReceivedEvent> _disruptor;
    private readonly ILogger<CandlestickPipeline> _logger;

    private readonly string _serviceName;
    private readonly bool _isEnabled;

    public CandlestickPipeline(
        IServiceProvider serviceProvider,
        ILogger<CandlestickPipeline> logger)
    {
        _logger = logger;
        _disruptor = ApplicationRegistrar.CreateCandlestickPipeline(serviceProvider);
        _serviceName = GetType().Name;
        _isEnabled = false; // get value from settings
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            if (!_isEnabled)
            {
                _logger.LogWarning($"{_serviceName} is disabled.");
                return;
            }

            _disruptor.Start();

            // TODO: Use channel reader to publish events
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        _disruptor.Shutdown();
        _logger.LogInformation($"{_serviceName} execution completed at {DateTime.UtcNow:O}");
    }
}
