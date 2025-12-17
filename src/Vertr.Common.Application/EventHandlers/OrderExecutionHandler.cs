using Disruptor;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Abstractions;

namespace Vertr.Common.Application.EventHandlers;

internal sealed class OrderExecutionHandler : IAsyncBatchEventHandler<CandleReceivedEvent>
{
    private readonly ILogger<OrderExecutionHandler> _logger;

    private readonly ITradingGateway _tinvestGateway;

    public OrderExecutionHandler(
        ITradingGateway tinvestGateway,
        ILogger<OrderExecutionHandler> logger)
    {
        _tinvestGateway = tinvestGateway;
        _logger = logger;
    }

    public async ValueTask OnBatch(EventBatch<CandleReceivedEvent> batch, long sequence)
    {
        try
        {
            var tasks = new List<Task>();

            foreach (var data in batch)
            {
                foreach (var request in data.OrderRequests)
                {
                    tasks.Add(_tinvestGateway.PostMarketOrder(request));
                }
            }

            await Task.WhenAll(tasks);
            _logger.LogInformation("OrderExecutionHandler executed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OrderExecutionHandler error. Message={Message}", ex.Message);
        }
    }
}
