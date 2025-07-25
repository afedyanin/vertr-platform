using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Application.CommandHandlers;
using Vertr.OrderExecution.Contracts.Commands;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.Platform.Common.Mediator;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.OrderExecution.Application.RequestHandlers;
internal class ClosePositionHandler : OrderHandlerBase, ICommandHandler<ClosePositionCommand, ExecuteOrderResponse>
{
    private readonly ICommandHandler<ExecuteOrderCommand, ExecuteOrderResponse> _executeOrderHandler;

    public ClosePositionHandler(
        ICommandHandler<ExecuteOrderCommand, ExecuteOrderResponse> executeOrderHandler,
        IPortfolioRepository portfolioRepository,
        IMarketInstrumentRepository staticMarketDataProvider
        ) : base(portfolioRepository, staticMarketDataProvider)
    {
        _executeOrderHandler = executeOrderHandler;
    }

    public async Task<ExecuteOrderResponse> Handle(
        ClosePositionCommand request,
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

        var orderRequest = new ExecuteOrderCommand
        {
            RequestId = request.RequestId,
            PortfolioIdentity = request.PortfolioIdentity,
            InstrumentId = request.InstrumentId,
            QtyLots = lotsToClose,
        };

        var response = await _executeOrderHandler.Handle(orderRequest, cancellationToken);

        var result = new ExecuteOrderResponse
        {
            OrderId = response.OrderId,
        };

        return result;
    }
}
