using Disruptor;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.EventHandlers;

internal sealed class PortfolioPositionHandler : IEventHandler<CandleReceivedEvent>
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

    public void OnEvent(CandleReceivedEvent data, long sequence, bool endOfBatch)
    {
        foreach (var signal in data.TradingSignals)
        {
            if (signal.Direction == TradingDirection.Hold)
            {
                continue;
            }

            var orderRequest = _portfolioManager.HandleTradingSignal(signal);

            if (orderRequest != null)
            {
                data.OrderRequests.Add(orderRequest);
            }
        }

        _logger.LogInformation("#{Sequence} PortfolioPositionHandler executed. {SignalsCount} signals added.", sequence, data.TradingSignals.Count);
    }
}
