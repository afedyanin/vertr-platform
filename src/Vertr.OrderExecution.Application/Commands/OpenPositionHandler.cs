using MediatR;
using Vertr.OrderExecution.Application.Abstractions;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.OrderExecution.Application.Commands;
internal class OpenPositionHandler : PositionHandlerBase, IRequestHandler<OpenPositionRequest, OrderExecutionResponse>
{
    public OpenPositionHandler(
        IMediator mediator,
        IPortfolioClient portfolioClient,
        IStaticMarketDataProvider staticMarketDataProvider
        ) : base(mediator, portfolioClient, staticMarketDataProvider)
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
