using MediatR;
using Vertr.OrderExecution.Application.Abstractions;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.OrderExecution.Application.Commands;
internal class ReversePositionHandler : PositionHandlerBase, IRequestHandler<ReversePositionRequest, OrderExecutionResponse>
{
    public ReversePositionHandler(
        IMediator mediator,
        IPortfolioClient portfolioClient,
        IStaticMarketDataProvider staticMarketDataProvider
        ) : base(mediator, portfolioClient, staticMarketDataProvider)
    {
    }

    public async Task<OrderExecutionResponse> Handle(
        ReversePositionRequest request,
        CancellationToken cancellationToken)
    {
        var currentLots = await GetCurrentPositionInLots(request.AccountId, request.InstrumentId);

        if (currentLots == 0L)
        {
            return new OrderExecutionResponse()
            {
                ErrorMessage = "Position closed."
            };
        }

        var lotsToRevert = currentLots * (-2L);

        var orderRequest = new PostOrderRequest
        {
            AccountId = request.AccountId,
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
