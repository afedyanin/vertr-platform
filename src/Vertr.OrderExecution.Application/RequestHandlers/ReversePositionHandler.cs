using MediatR;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.OrderExecution.Application.RequestHandlers;

internal class ReversePositionHandler : PositionHandlerBase, IRequestHandler<ReversePositionRequest, OrderExecutionResponse>
{
    public ReversePositionHandler(
        IMediator mediator,
        IPortfolioManager portfolioManager,
        IMarketDataService marketDataService
        ) : base(mediator, portfolioManager, marketDataService)
    {
    }

    public async Task<OrderExecutionResponse> Handle(
        ReversePositionRequest request,
        CancellationToken cancellationToken)
    {
        var currentLots = await GetCurrentPositionInLots(request.PortfolioId, request.InstrumentId);

        if (currentLots == 0L)
        {
            return new OrderExecutionResponse()
            {
                ErrorMessage = "Position closed."
            };
        }

        var lotsToRevert = currentLots * -2L;

        var orderRequest = new PostOrderRequest
        {
            PortfolioId = request.PortfolioId,
            InstrumentId = request.InstrumentId,
            RequestId = request.RequestId,
            QtyLots = lotsToRevert,
        };

        var response = await Mediator.Send(orderRequest, cancellationToken);

        var result = new OrderExecutionResponse
        {
            OrderId = response.OrderId,
        };

        return result;
    }
}
