using Microsoft.Extensions.Logging;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Abstractions.Handlers;

public class PortfolioPositionHandler<TEvent> : IEventHandler<TEvent> where TEvent : IMarketDataEvent
{
    private readonly IPortfolioManager _portfolioManager;
    private readonly ILogger _logger;

    public int HandlingOrder => 800;

    public PortfolioPositionHandler(
        IPortfolioManager portfolioManager,
        ILoggerFactory loggerFactory)
    {
        _portfolioManager = portfolioManager;
        _logger = loggerFactory.CreateLogger("PortfolioPositionHandler");
    }

    public ValueTask OnEvent(TEvent data)
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

        _logger.LogDebug("#{Sequence} PortfolioPositionHandler executed. {OrdersCount} order requests added.", data.Sequence, data.OrderRequests.Count);
        return ValueTask.CompletedTask;
    }
}
