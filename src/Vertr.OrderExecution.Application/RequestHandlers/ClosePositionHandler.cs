using MediatR;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.OrderExecution.Application.RequestHandlers;
internal class ClosePositionHandler : PositionHandlerBase, IRequestHandler<ClosePositionRequest, OrderExecutionResponse>
{
    public ClosePositionHandler(
        IMediator mediator,
        IPortfolioManager portfolioManager,
        IMarketDataService marketDataService
        ) : base(mediator, portfolioManager, marketDataService)
    {
    }

    public async Task<OrderExecutionResponse> Handle(
        ClosePositionRequest request,
        CancellationToken cancellationToken)
    {
        var currentLots = await GetCurrentPositionInLots(request.PortfolioId, request.InstrumentId);

        if (currentLots == 0L)
        {
            return new OrderExecutionResponse()
            {
                ErrorMessage = "Position already closed."
            };
        }

        var lotsToClose = currentLots * -1L;

        var orderRequest = new PostOrderRequest
        {
            PortfolioId = request.PortfolioId,
            InstrumentId = request.InstrumentId,
            RequestId = request.RequestId,
            QtyLots = lotsToClose,
        };

        var response = await Mediator.Send(orderRequest, cancellationToken);

        var result = new OrderExecutionResponse
        {
            OrderId = response.OrderId,
        };

        return result;
    }
}
