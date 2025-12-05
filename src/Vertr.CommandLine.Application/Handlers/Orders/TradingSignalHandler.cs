using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.Abstracttions;
using Vertr.CommandLine.Models.Requests.Orders;
using Vertr.Common.Mediator;

namespace Vertr.CommandLine.Application.Handlers.Orders;

public class TradingSignalHandler : IRequestHandler<TradingSignalRequest, TradingSignalResponse>
{
    private readonly IPortfolioService _portfolioService;
    private readonly IOrderExecutionService _orderExecutionService;
    private readonly IMarketDataService _marketDataService;

    public TradingSignalHandler(
        IPortfolioService portfolioService,
        IOrderExecutionService orderExecutionService,
        IMarketDataService marketDataService)
    {
        _portfolioService = portfolioService;
        _orderExecutionService = orderExecutionService;
        _marketDataService = marketDataService;
    }

    public async Task<TradingSignalResponse> Handle(TradingSignalRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Direction == Direction.Hold)
        {
            return new TradingSignalResponse()
            {
                Message = "Ignore signal with Hold direction."
            };
        }

        var position = _portfolioService.GetPosition(request.PortfolioId, request.Symbol);

        var nextMarketPrice = await _marketDataService.GetMarketPrice(request.Symbol, request.MarketTime, PriceType.Open, shift: 1);

        if (nextMarketPrice == null)
        {
            return new TradingSignalResponse()
            {
                Message = "Cannot get next market price to post order. Skipping signal."
            };
        }

        // Open position
        if (position == null || position.Qty == 0)
        {
            var qty = request.OpenPositionQty * (request.Direction == Direction.Buy ? 1 : -1);

            var trades = await _orderExecutionService.PostOrder(
                request.Symbol,
                qty,
                nextMarketPrice.Value,
                request.ComissionPercent);

            return new TradingSignalResponse()
            {
                Trades = trades,
            };
        }

        // Same direction
        if ((position.Qty > 0 && request.Direction == Direction.Buy) ||
            (position.Qty < 0 && request.Direction == Direction.Sell))
        {
            return new TradingSignalResponse()
            {
                Message = "Position is already opened with signal direction."
            };
        }

        // Reverse
        var reverseQty = position.Qty * (-2);

        var reverseTrades = await _orderExecutionService.PostOrder(
            request.Symbol,
            reverseQty,
            nextMarketPrice.Value,
            request.ComissionPercent);

        return new TradingSignalResponse()
        {
            Trades = reverseTrades,
        };
    }
}