using Disruptor;
using Microsoft.Extensions.Logging;

namespace Vertr.Common.Application.EventHandlers;

internal sealed class OrderExecutionHandler : IEventHandler<CandlestickReceivedEvent>
{
    private readonly ILogger<OrderExecutionHandler> _logger;


    public OrderExecutionHandler(ILogger<OrderExecutionHandler> logger)
    {
        _logger = logger;
    }

    public void OnEvent(CandlestickReceivedEvent data, long sequence, bool endOfBatch)
    {
        // Send order requests to execution engine

        _logger.LogInformation("OrderExecutionHandler executed.");
    }
}
