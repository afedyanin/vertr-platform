using MediatR;
using Vertr.OrderExecution.Application.Abstractions;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.OrderExecution.Application.Commands;
internal class OpenPositionHandler : PositionHandlerBase, IRequestHandler<OpenPositionRequest, OpenPositionResponse>
{
    public OpenPositionHandler(
        IMediator mediator,
        IPortfolioClient portfolioClient,
        IStaticMarketDataProvider staticMarketDataProvider
        ) : base(mediator, portfolioClient, staticMarketDataProvider)
    {
    }

    public async Task<OpenPositionResponse> Handle(OpenPositionRequest request, CancellationToken cancellationToken)
    {
        var currentLots = await GetCurrentPositionInLots(request.AccountId, request.InstrumentId);

        if (currentLots != 0L)
        {
            return new OpenPositionResponse()
            {
                Message = "Position already opened."
            };
        }

        var orderRequest = new PostOrderRequest
        {
            AccountId = request.AccountId,
            InstrumentId = request.InstrumentId,
            RequestId = request.RequestId,
            QtyLots = request.QtyLots,
        };

        var response = await Mediator.Send(orderRequest, cancellationToken);

        var result = new OpenPositionResponse
        {
            PostOrderResult = response.PostOrderResult,
        };

        return result;
    }
}
