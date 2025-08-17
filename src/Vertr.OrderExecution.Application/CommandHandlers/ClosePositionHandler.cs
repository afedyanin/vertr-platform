using Microsoft.Extensions.Options;
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
        IInstrumentsRepository staticMarketDataProvider,
        IOptions<OrderExecutionSettings> options) :
        base(mediator, portfolioRepository, staticMarketDataProvider, options)
    {
    }

    public async Task<ExecuteOrderResponse> Handle(
        ClosePositionRequest request,
        CancellationToken cancellationToken)
    {
        var currentLots = await GetCurrentPositionInLots(request.SubAccountId, request.InstrumentId);

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
            SubAccountId = request.SubAccountId,
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
