using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Abstractions;
using Vertr.Strategies.FuturesArbitrage.Abstractions;

namespace Vertr.Strategies.FuturesArbitrage.Services;

internal sealed class FuturesProcessingPipeline : IFuturesProcessingPipeline
{
    private readonly ILogger<FuturesProcessingPipeline> _logger;
    private readonly IEventHandler<OrderBookChangedEvent>[] _pipeline = [];

    public FuturesProcessingPipeline(
        IEnumerable<IEventHandler<OrderBookChangedEvent>> eventHandlers,
        ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<FuturesProcessingPipeline>();
        _pipeline = [.. eventHandlers.OrderBy(h => h.HandlingOrder)];
    }

    public async Task Handle(OrderBookChangedEvent tEvent)
    {
        try
        {
            foreach (var handler in _pipeline)
            {
                await handler.OnEvent(tEvent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing event={Event} Message={Message}", tEvent, ex.Message);
        }
    }

    public Task Start(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
