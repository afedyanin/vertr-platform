using Disruptor;
using Microsoft.Extensions.Logging;

namespace Vertr.Common.Application.EventHandlers;

internal class MarketDataPredictor : IEventHandler<CandlestickReceivedEvent>
{
    private readonly ILogger<MarketDataPredictor> _logger;

    public MarketDataPredictor(ILogger<MarketDataPredictor> logger)
    {
        _logger = logger;
    }

    public void OnEvent(CandlestickReceivedEvent data, long sequence, bool endOfBatch)
    {
        // Collect required market data
        // Get Predictions from prediction engine.
        // Save predictions to event data

        _logger.LogInformation("MarketDataHandler executed.");
    }
}
