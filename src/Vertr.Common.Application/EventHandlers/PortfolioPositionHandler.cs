using Disruptor;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Services;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.EventHandlers;

internal sealed class PortfolioPositionHandler : IAsyncBatchEventHandler<CandlestickReceivedEvent>
{
    private readonly IPortfolioManager _portfolioManager;
    private readonly ILogger<PortfolioPositionHandler> _logger;

    public PortfolioPositionHandler(
        IPortfolioManager portfolioManager,
        ILogger<PortfolioPositionHandler> logger)
    {
        _portfolioManager = portfolioManager;
        _logger = logger;
    }

    public async ValueTask OnBatch(EventBatch<CandlestickReceivedEvent> batch, long sequence)
    {
        _logger.LogInformation("Start processing PortfolioPosition Sequence={Sequence}", sequence);

        try
        {
            foreach (var data in batch)
            {
                foreach (var signal in data.TradingSignals)
                {
                    if (signal.Direction == TradingDirection.Hold)
                    {
                        continue;
                    }

                    var orderRequest = await _portfolioManager.HandleTradingSignal(signal);

                    if (orderRequest != null)
                    {
                        data.OrderRequests.Add(orderRequest);
                    }
                }
            }

            _logger.LogInformation("PortfolioPositionHandler executed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PortfolioPositionHandler error. Message={Message}", ex.Message);
        }
    }
}
