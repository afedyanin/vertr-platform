using MediatR;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.OrderExecution.Application.RequestHandlers;
internal class OpenPositionHandler : PositionHandlerBase, IRequestHandler<OpenPositionRequest, ExecuteOrderResponse>
{
    public OpenPositionHandler(
        IMediator mediator,
        IPortfolioRepository portfolioRepository,
        IStaticMarketDataProvider staticMarketDataProvider
        ) : base(mediator, portfolioRepository, staticMarketDataProvider)
    {
    }

    public async Task<ExecuteOrderResponse> Handle(OpenPositionRequest request, CancellationToken cancellationToken)
    {
        var currentLots = await GetCurrentPositionInLots(request.PortfolioIdentity, request.InstrumentIdentity);

        if (currentLots != 0L)
        {
            return new ExecuteOrderResponse()
            {
                ErrorMessage = "Position already opened."
            };
        }

        var orderRequest = new ExecuteOrderRequest
        {
            RequestId = request.RequestId,
            PortfolioIdentity = request.PortfolioIdentity,
            InstrumentIdentity = request.InstrumentIdentity,
            QtyLots = request.QtyLots,
        };

        var response = await Mediator.Send(orderRequest, cancellationToken);

        var result = new ExecuteOrderResponse
        {
            OrderId = response.OrderId,
        };

        return result;
    }
}
