using Disruptor;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Services;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.EventHandlers;

internal sealed class PortfolioPositionHandler : IEventHandler<CandlestickReceivedEvent>
{
    private readonly ILogger<PortfolioPositionHandler> _logger;

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
        // TODO: Handle signal & generate order request

        var portfolio = _portfolioService.GetAll().FirstOrDefault();

        if (portfolio == null)
        {
            portfolio = new Portfolio()
            {
                Id = Guid.NewGuid(),
                UpdatedAt = DateTime.UtcNow,
            };

            _portfolioService.Update(portfolio);
        }

        // TODO: Implement reverse position
        var request = new MarketOrderRequest
        {
            RequestId = Guid.NewGuid(),
            InstrumentId = data.Candle!.InstrumentId,
            PortfolioId = portfolio.Id,
            QuantityLots = 10 * GetOrderDirection(),
        };

        data.OrderRequests.Add(request);

        _logger.LogInformation("PortfolioPositionHandler executed.");
    }

    private static int GetOrderDirection() => Random.Shared.Next(0, 2) > 0 ? 1 : -1;
}
