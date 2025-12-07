using Disruptor;
using Microsoft.Extensions.Logging;

namespace Vertr.Common.Application.EventHandlers;

internal class MarketDataHandler : IEventHandler<CandlestickReceivedEvent>
{
    private readonly ILogger<MarketDataHandler> _logger;

    public MarketDataHandler(ILogger<MarketDataHandler> logger)
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
