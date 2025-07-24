using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Commands;
using Vertr.Platform.Common;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.OrderExecution.Application.CommandHandlers;

internal class ReversePositionHandler : OrderHandlerBase, ICommandHandler<ReversePositionCommand, ExecuteOrderResponse>
{
    private readonly ICommandHandler<ExecuteOrderCommand, ExecuteOrderResponse> _executeOrderHandler;

    public ReversePositionHandler(
        ICommandHandler<ExecuteOrderCommand, ExecuteOrderResponse> executeOrderHandler,
        IPortfolioRepository portfolioRepository,
        IMarketInstrumentRepository staticMarketDataProvider
        ) : base(portfolioRepository, staticMarketDataProvider)
    {
        _executeOrderHandler = executeOrderHandler;
    }

    public async Task<ExecuteOrderResponse> Handle(
        ReversePositionCommand request,
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

        var orderRequest = new ExecuteOrderCommand
        {
            RequestId = request.RequestId,
            PortfolioIdentity = request.PortfolioIdentity,
            InstrumentId = request.InstrumentId,
            QtyLots = lotsToRevert,
        };

        var response = await _executeOrderHandler.Handle(orderRequest, cancellationToken);

        var result = new ExecuteOrderResponse
        {
            OrderId = response.OrderId,
        };

        return result;
    }
}
