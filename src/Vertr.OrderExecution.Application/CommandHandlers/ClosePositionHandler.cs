using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Commands;
using Vertr.Platform.Common.Mediator;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.OrderExecution.Application.CommandHandlers;

internal class ClosePositionHandler : OrderHandlerBase, IRequestHandler<ClosePositionRequest, ExecuteOrderResponse>
{
    public ClosePositionHandler(
        IMediator mediator,
        IPortfolioRepository portfolioRepository,
        IInstrumentsRepository staticMarketDataProvider
        ) : base(mediator, portfolioRepository, staticMarketDataProvider)
    {
    }

    public async Task<ExecuteOrderResponse> Handle(
        ClosePositionRequest request,
        CancellationToken cancellationToken)
    {
        var currentLots = await GetCurrentPositionInLots(request.PortfolioIdentity, request.InstrumentId);

        if (currentLots == 0L)
        {
            return new ExecuteOrderResponse()
            {
                ErrorMessage = "Position already closed."
            };
        }

        var lotsToClose = currentLots * -1L;

        var orderRequest = new ExecuteOrderRequest
        {
            RequestId = request.RequestId,
            PortfolioIdentity = request.PortfolioIdentity,
            InstrumentId = request.InstrumentId,
            QtyLots = lotsToClose,
        };

        var response = await Mediator.Send(orderRequest, cancellationToken);

        var result = new ExecuteOrderResponse
        {
            OrderId = response.OrderId,
        };

        return result;
    }
}
