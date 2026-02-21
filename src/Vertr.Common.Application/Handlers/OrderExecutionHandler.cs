
using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Abstractions;

namespace Vertr.Common.Application.Handlers;

public class OrderExecutionHandler<TEvent> : IEventHandler<TEvent> where TEvent : IMarketDataEvent
{
    private readonly ITradingGateway _tinvestGateway;
    private readonly ILogger _logger;

    public int HandlingOrder => 900;

    public OrderExecutionHandler(
        ITradingGateway tinvestGateway,
        ILoggerFactory loggerFactory)
    {
        _tinvestGateway = tinvestGateway;
        _logger = loggerFactory.CreateLogger("OrderExecutionHandler");
    }

    public async ValueTask OnEvent(TEvent data)
    {
        try
        {
            foreach (var request in data.OrderRequests)
            {
                _logger.LogInformation("#{Sequence} Posting request: {Request}.", data.Sequence, request);
                request.OrderId = await _tinvestGateway.PostMarketOrder(request);
            }

            _logger.LogDebug("#{Sequence} OrderExecutionHandler executed.", data.Sequence);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OrderExecutionHandler error. Message={Message}", ex.Message);
        }
    }
}
