using Disruptor;
using Microsoft.Extensions.Logging;

namespace Vertr.Common.Application.EventHandlers;

internal class PortfolioPositionHandler : IEventHandler<CandlestickReceivedEvent>
{
    private readonly ILogger<PortfolioPositionHandler> _logger;

    public PortfolioPositionHandler(ILogger<PortfolioPositionHandler> logger)
    {
        _logger = logger;
    }

    public void OnEvent(CandlestickReceivedEvent data, long sequence, bool endOfBatch)
    {
        // Convert trading signals to order requests for each portfolio

        _logger.LogInformation("RiskEngineHandler executed.");
    }
}
