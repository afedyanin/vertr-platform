
using Microsoft.Extensions.Logging;

namespace Vertr.Common.Application.Abstractions.Handlers;

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
            // var tasks = new List<Task>();

            foreach (var request in data.OrderRequests)
            {
                // tasks.Add(_tinvestGateway.PostMarketOrder(request!));
                _logger.LogInformation("#{Sequence} Posting request: {Request}.", data.Sequence, request);
                await _tinvestGateway.PostMarketOrder(request);
            }

            // await Task.WhenAll(tasks);

            _logger.LogDebug("#{Sequence} OrderExecutionHandler executed.", data.Sequence);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OrderExecutionHandler error. Message={Message}", ex.Message);
        }
    }
}
