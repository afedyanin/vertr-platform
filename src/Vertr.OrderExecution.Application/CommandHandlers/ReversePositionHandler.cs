using Microsoft.Extensions.Options;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Commands;
using Vertr.Platform.Common.Mediator;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.OrderExecution.Application.CommandHandlers;

internal class ReversePositionHandler : OrderHandlerBase, IRequestHandler<ReversePositionRequest, ExecuteOrderResponse>
{
    public ReversePositionHandler(
        IMediator mediator,
        IPortfolioProvider portfolioProvider,
        IInstrumentsRepository staticMarketDataProvider,
        IOptions<OrderExecutionSettings> options) :
        base(mediator, portfolioProvider, staticMarketDataProvider, options)
    {
    }

    public async Task<ExecuteOrderResponse> Handle(
        ReversePositionRequest request,
        CancellationToken cancellationToken)
    {
        var currentLots = await GetCurrentPositionInLots(request.SubAccountId, request.InstrumentId);

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
            SubAccountId = request.SubAccountId,
            InstrumentId = request.InstrumentId,
            QtyLots = lotsToRevert,
            CreatedAt = request.CreatedAt,
        };

        var response = await Mediator.Send(orderRequest, cancellationToken);

        var result = new ExecuteOrderResponse
        {
            OrderId = response.OrderId,
        };

        return result;
    }
}
