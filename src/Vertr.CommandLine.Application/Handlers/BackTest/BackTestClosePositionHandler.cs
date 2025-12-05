using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models.Abstracttions;
using Vertr.CommandLine.Models.Requests.BackTest;

namespace Vertr.CommandLine.Application.Handlers.BackTest;

public class BackTestClosePositionHandler : IRequestHandler<BackTestClosePositionRequest, BackTestClosePostionResponse>
{
    private readonly IPortfolioService _portfolioService;
    private readonly IOrderExecutionService _orderExecutionService;
    private readonly IMarketDataService _marketDataService;

    public BackTestClosePositionHandler(
        IPortfolioService portfolioService,
        IOrderExecutionService orderExecutionService,
        IMarketDataService marketDataService)
    {
        _portfolioService = portfolioService;
        _orderExecutionService = orderExecutionService;
        _marketDataService = marketDataService;
    }

    public async Task<BackTestClosePostionResponse> Handle(
        BackTestClosePositionRequest request,
        CancellationToken cancellationToken = default)
    {
        var position = _portfolioService.GetPosition(request.PortfolioId, request.Symbol);

        var items = new Dictionary<string, object>
        {
            [BackTestContextKeys.MarketTime] = request.MarketTime
        };

        if (position == null || position.Qty == 0)
        {
            var message = $"Position with PortfolioId={request.PortfolioId} with Symbol={request.Symbol} is already closed.";

            items[BackTestContextKeys.Message] = message;

            return new BackTestClosePostionResponse
            {
                Message = message,
                Items = items
            };
        }

        var marketPrice = await _marketDataService.GetMarketPrice(request.Symbol, request.MarketTime, Models.PriceType.Open);

        if (marketPrice == null)
        {
            var message = $"Cannot get market price to post order. MarketTime={request.MarketTime} Skipping request.";

            items[BackTestContextKeys.Message] = message;

            return new BackTestClosePostionResponse()
            {
                Message = message,
                Items = items
            };
        }

        var trades = await _orderExecutionService.PostOrder(
            request.Symbol,
            position.Qty * (-1),
            marketPrice.Value,
            request.ComissionPercent);

        if (trades == null || trades.Length <= 0)
        {
            var message = $"No trades received for close PortfolioId={request.PortfolioId} with Symbol={request.Symbol}.";

            items[BackTestContextKeys.Message] = message;

            return new BackTestClosePostionResponse
            {
                Message = message,
                Items = items
            };
        }

        var positions = _portfolioService.Update(
            request.PortfolioId,
            request.Symbol,
            trades,
            request.CurrencyCode);

        items[BackTestContextKeys.Trades] = trades;
        items[BackTestContextKeys.Positions] = positions;

        return new BackTestClosePostionResponse
        {
            Items = items
        };
    }
}