using MediatR;
using Vertr.OrderExecution.Application.Abstractions;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.OrderExecution.Application.Commands;
internal class ClosePositionHandler : PositionHandlerBase, IRequestHandler<ClosePositionRequest, ClosePositionResponse>
{
    public ClosePositionHandler(
        IMediator mediator,
        IPortfolioClient portfolioClient,
        IStaticMarketDataProvider staticMarketDataProvider
        ) : base(mediator, portfolioClient, staticMarketDataProvider)
    {
    }

    public async Task<ClosePositionResponse> Handle(
        ClosePositionRequest request,
        CancellationToken cancellationToken)
    {
        var currentLots = await GetCurrentPositionInLots(request.AccountId, request.InstrumentId);

        if (currentLots == 0L)
        {
            return new ClosePositionResponse()
            {
                Message = "Position already closed."
            };
        }

        var lotsToClose = currentLots * (-1L);

        var orderRequest = new PostOrderRequest
        {
            AccountId = request.AccountId,
            InstrumentId = request.InstrumentId,
            RequestId = request.RequestId,
            QtyLots = lotsToClose,
        };

        var response = await Mediator.Send(orderRequest, cancellationToken);

        var result = new ClosePositionResponse
        {
            PostOrderResult = response.PostOrderResult,
        };

        return result;
    }
}
