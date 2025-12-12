using Disruptor;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Services;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.EventHandlers;

internal sealed class PortfolioPositionHandler : IEventHandler<CandlestickReceivedEvent>
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

    public void OnEvent(CandlestickReceivedEvent data, long sequence, bool endOfBatch)
    {
        _logger.LogInformation("Start processing PortfolioPosition Sequence={Sequence}", sequence);

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

        _logger.LogInformation("PortfolioPositionHandler executed.");
    }
}
