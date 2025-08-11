using Vertr.Platform.Common.Mediator;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Commands;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.OrderExecution.Application.CommandHandlers;

internal class OpenPositionHandler : OrderHandlerBase, IRequestHandler<OpenPositionCommand, ExecuteOrderResponse>
{
    public OpenPositionHandler(
        IMediator mediator,
        IPortfolioRepository portfolioRepository,
        IInstrumentsRepository staticMarketDataProvider
        ) : base(mediator, portfolioRepository, staticMarketDataProvider)
    {
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

        var response = await Mediator.Send(orderRequest, cancellationToken);

        var result = new ExecuteOrderResponse
        {
            OrderId = response.OrderId,
        };

        return result;
    }
}
