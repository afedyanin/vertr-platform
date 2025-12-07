using Disruptor;
using Microsoft.Extensions.Logging;

namespace Vertr.Common.Application.EventHandlers;

internal class RiskEngineHandler : IEventHandler<CandlestickReceivedEvent>
{
    private readonly ILogger<RiskEngineHandler> _logger;

    public RiskEngineHandler(ILogger<RiskEngineHandler> logger)
    {
        _logger = logger;
    }

    public void OnEvent(CandlestickReceivedEvent data, long sequence, bool endOfBatch)
    {
        // Convert trading signals to order requests for each portfolio

        _logger.LogInformation("RiskEngineHandler executed.");
    }
}
