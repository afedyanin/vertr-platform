using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Handlers;

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

            var orderRequest = _portfolioManager.HandleTradingSignal(signal, reverseOnly: false);

            if (orderRequest != null)
            {
                data.OrderRequests.Add(orderRequest);
                continue;
            }

            // force reverse signal by basic asset
            // если открытая позиция производного актива не соответствует направлению сингала базового актива
            // разворачиваем ее независимо от цен и стакана производного актива
            if (data.TradingDirection != TradingDirection.Hold)
            {
                var reverseSignal = new TradingSignal
                {
                    Direction = data.TradingDirection,
                    Instrument = signal.Instrument,
                    PortfolioName = signal.PortfolioName,
                };

                var reverseOrderRequest = _portfolioManager.HandleTradingSignal(reverseSignal, reverseOnly: true);

                if (reverseOrderRequest != null)
                {
                    data.OrderRequests.Add(reverseOrderRequest);
                }
            }
        }

        if (data.OrderRequests.Any())
        {
            _logger.LogInformation("#{Sequence} PortfolioPositionHandler executed. {OrdersCount} order requests added.", data.Sequence, data.OrderRequests.Count);
        }

        return ValueTask.CompletedTask;
    }
}
