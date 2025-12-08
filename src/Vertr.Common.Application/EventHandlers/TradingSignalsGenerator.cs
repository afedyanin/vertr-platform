using Disruptor;
using Microsoft.Extensions.Logging;

namespace Vertr.Common.Application.EventHandlers;

internal class TradingSignalsGenerator : IEventHandler<CandlestickReceivedEvent>
{
    private readonly ILogger<TradingSignalsGenerator> _logger;

    public TradingSignalsGenerator(ILogger<TradingSignalsGenerator> logger)
    {
        _logger = logger;
    }

    public void OnEvent(CandlestickReceivedEvent data, long sequence, bool endOfBatch)
    {
        // Get current market price
        // Evaluate prediction & thresholds
        // Generate and save trading signals

        _logger.LogInformation("PredictionHandler executed.");
    }
}
