using Vertr.Platform.Common.Mediator;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Commands;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.OrderExecution.Application.CommandHandlers;

internal class ReversePositionHandler : OrderHandlerBase, IRequestHandler<ReversePositionRequest, ExecuteOrderResponse>
{
    public ReversePositionHandler(
        IMediator mediator,
        IPortfolioRepository portfolioRepository,
        IInstrumentsRepository staticMarketDataProvider
        ) : base(mediator, portfolioRepository, staticMarketDataProvider)
    {
    }

    public async Task<ExecuteOrderResponse> Handle(
        ReversePositionRequest request,
        CancellationToken cancellationToken)
    {
        var currentLots = await GetCurrentPositionInLots(request.PortfolioIdentity, request.InstrumentId);

        if (currentLots == 0L)
        {
            return new ExecuteOrderResponse()
            {
                ErrorMessage = "Position closed."
            };
        }

        var lotsToRevert = currentLots * -2L;

        var orderRequest = new ExecuteOrderRequest
        {
            RequestId = request.RequestId,
            PortfolioIdentity = request.PortfolioIdentity,
            InstrumentId = request.InstrumentId,
            QtyLots = lotsToRevert,
        };

        var response = await Mediator.Send(orderRequest, cancellationToken);

        var result = new ExecuteOrderResponse
        {
            OrderId = response.OrderId,
        };

        return result;
    }
}
