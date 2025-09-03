using Microsoft.Extensions.Options;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Commands;
using Vertr.Platform.Common.Mediator;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.OrderExecution.Application.CommandHandlers;

internal class OpenPositionHandler : OrderHandlerBase, IRequestHandler<OpenPositionCommand, ExecuteOrderResponse>
{
    public OpenPositionHandler(
        IMediator mediator,
        IPortfolioRepository portfolioProvider,
        IInstrumentsRepository staticMarketDataProvider,
        IOptions<OrderExecutionSettings> options
        ) : base(mediator, portfolioProvider, staticMarketDataProvider, options)
    {
    }

    public async Task<ExecuteOrderResponse> Handle(OpenPositionCommand request, CancellationToken cancellationToken)
    {
        var currentLots = await GetCurrentPositionInLots(request.PortfolioId, request.InstrumentId);

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
            BacktestId = request.BacktestId,
            PortfolioId = request.PortfolioId,
            InstrumentId = request.InstrumentId,
            QtyLots = request.QtyLots,
            CreatedAt = request.CreatedAt,
            Price = request.Price,
        };

        var response = await Mediator.Send(orderRequest, cancellationToken);

        var result = new ExecuteOrderResponse
        {
            OrderId = response.OrderId,
        };

        return result;
    }
}
