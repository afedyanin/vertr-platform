using MediatR;
using Vertr.OrderExecution.Application.Abstractions;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.OrderExecution.Application.Commands;
internal class ClosePositionHandler : PositionHandlerBase, IRequestHandler<ClosePositionRequest, OrderExecutionResponse>
{
    public ClosePositionHandler(
        IMediator mediator,
        IPortfolioClient portfolioClient,
        IStaticMarketDataProvider staticMarketDataProvider
        ) : base(mediator, portfolioClient, staticMarketDataProvider)
    {
    }

    public async Task<OrderExecutionResponse> Handle(
        ClosePositionRequest request,
        CancellationToken cancellationToken)
    {
        var currentLots = await GetCurrentPositionInLots(request.AccountId, request.InstrumentId);

        if (currentLots == 0L)
        {
            return new OrderExecutionResponse()
            {
                ErrorMessage = "Position already closed."
            };
        }

        var lotsToClose = currentLots * (-1L);

        var orderRequest = new PostOrderRequest
        {
            AccountId = request.AccountId,
            InstrumentId = request.InstrumentId,
            RequestId = request.RequestId,
            QtyLots = lotsToClose,
            BookId = request.BookId,
        };

        var response = await Mediator.Send(orderRequest, cancellationToken);

        var result = new OrderExecutionResponse
        {
            OrderId = response.OrderId,
        };

        return result;
    }
}
