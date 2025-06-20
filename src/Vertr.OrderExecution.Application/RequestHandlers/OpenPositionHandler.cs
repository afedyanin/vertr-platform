using MediatR;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.OrderExecution.Application.RequestHandlers;
internal class OpenPositionHandler : PositionHandlerBase, IRequestHandler<OpenPositionRequest, OrderExecutionResponse>
{
    public OpenPositionHandler(
        IMediator mediator,
        IPortfolioManager portfolioManager,
        IMarketDataService marketDataService
        ) : base(mediator, portfolioManager, marketDataService)
    {
    }

    public async Task<OrderExecutionResponse> Handle(OpenPositionRequest request, CancellationToken cancellationToken)
    {
        var currentLots = await GetCurrentPositionInLots(request.PortfolioId, request.InstrumentId);

        if (currentLots != 0L)
        {
            return new OrderExecutionResponse()
            {
                ErrorMessage = "Position already opened."
            };
        }

        var orderRequest = new PostOrderRequest
        {
            PortfolioId = request.PortfolioId,
            InstrumentId = request.InstrumentId,
            RequestId = request.RequestId,
            QtyLots = request.QtyLots,
        };

        var response = await Mediator.Send(orderRequest, cancellationToken);

        var result = new OrderExecutionResponse
        {
            OrderId = response.OrderId,
        };

        return result;
    }
}
