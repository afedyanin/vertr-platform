using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Commands;
using Vertr.Platform.Common;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.OrderExecution.Application.CommandHandlers;

internal class OpenPositionHandler : OrderHandlerBase, ICommandHandler<OpenPositionCommand, ExecuteOrderResponse>
{
    private readonly ICommandHandler<ExecuteOrderCommand, ExecuteOrderResponse> _executeOrderHandler;

    public OpenPositionHandler(
        ICommandHandler<ExecuteOrderCommand, ExecuteOrderResponse> executeOrderHandler,
        IPortfolioRepository portfolioRepository,
        IStaticMarketDataProvider staticMarketDataProvider
        ) : base( portfolioRepository, staticMarketDataProvider)
    {
        _executeOrderHandler = executeOrderHandler;
    }

    public async Task<ExecuteOrderResponse> Handle(OpenPositionCommand request, CancellationToken cancellationToken)
    {
        var currentLots = await GetCurrentPositionInLots(request.PortfolioIdentity, request.InstrumentId);

        if (currentLots != 0L)
        {
            return new ExecuteOrderResponse()
            {
                ErrorMessage = "Position already opened."
            };
        }

        var orderRequest = new ExecuteOrderCommand
        {
            RequestId = request.RequestId,
            PortfolioIdentity = request.PortfolioIdentity,
            InstrumentId = request.InstrumentId,
            QtyLots = request.QtyLots,
        };

        var response = await _executeOrderHandler.Handle(orderRequest, cancellationToken);

        var result = new ExecuteOrderResponse
        {
            OrderId = response.OrderId,
        };

        return result;
    }
}
