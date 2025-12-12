using Disruptor;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Services;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.EventHandlers;

internal sealed class PortfolioPositionHandler : IEventHandler<CandlestickReceivedEvent>
{
    private readonly ILogger<PortfolioPositionHandler> _logger;

    // TODO: Get from settings
    private const int DefaultQtyLots = 10;

    private readonly IPortfolioService _portfolioService;
    public PortfolioPositionHandler(
        IPortfolioService portfolioService,
        ILogger<PortfolioPositionHandler> logger)
    {
        _portfolioService = portfolioService;
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

            var portfolio = _portfolioService.GetByPredictor(signal.Predictor);

            if (portfolio == null)
            {
                _logger.LogWarning("Portfolio is not found for predictor={Predictor}", signal.Predictor);
                continue;
            }

            var position = portfolio.Positions.FirstOrDefault(p => p.InstrumentId == signal.InstrumentId);

            if (position.Amount > 0 && signal.Direction == TradingDirection.Buy)
            {
                continue;
            }

            if (position.Amount < 0 && signal.Direction == TradingDirection.Sell)
            {
                continue;
            }

            // Open position
            if (position.Amount == default)
            {
                var openRequest = new MarketOrderRequest
                {
                    RequestId = Guid.NewGuid(),
                    InstrumentId = signal.InstrumentId,
                    PortfolioId = portfolio.Id,
                    QuantityLots = DefaultQtyLots,
                };

                _logger.LogInformation("Open position Request: QuantityLots={QuantityLots}", openRequest.QuantityLots);
                data.OrderRequests.Add(openRequest);
                continue;
            }

            // Reverse position
            var reverseDirection = position.Amount > 0 ? (-2) : 2;
            var reverseRequest = new MarketOrderRequest
            {
                RequestId = Guid.NewGuid(),
                InstrumentId = signal.InstrumentId,
                PortfolioId = portfolio.Id,
                QuantityLots = DefaultQtyLots * reverseDirection,
            };

            _logger.LogInformation("Reverse position Request: QuantityLots={QuantityLots}", reverseRequest.QuantityLots);
            data.OrderRequests.Add(reverseRequest);
        }

        _logger.LogInformation("PortfolioPositionHandler executed.");
    }
}
