using MediatR;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Commands;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.OrderExecution.Application.CommandHandlers;

internal class TradingSignalHandler : OrderHandlerBase, IRequestHandler<TradingSignalCommand, ExecuteOrderResponse>
{
    public TradingSignalHandler(
        IMediator mediator,
        IPortfolioRepository portfolioRepository,
        IInstrumentsRepository staticMarketDataProvider
        ) : base(mediator, portfolioRepository, staticMarketDataProvider)
    {
    }

    public async Task<ExecuteOrderResponse> Handle(
        TradingSignalCommand request,
        CancellationToken cancellationToken)
    {
        if (request.QtyLots == 0L)
        {
            return new ExecuteOrderResponse
            {
                ErrorMessage = "Trading signal quantity is empty."
            };
        }

        var currentLots = await GetCurrentPositionInLots(request.PortfolioIdentity, request.InstrumentId);

        if (currentLots == 0L)
        {
            var openRequest = new OpenPositionCommand
            {
                RequestId = request.RequestId,
                PortfolioIdentity = request.PortfolioIdentity,
                InstrumentId = request.InstrumentId,
                QtyLots = request.QtyLots,
            };

            var openResponse = await Mediator.Send(openRequest, cancellationToken);

            return new ExecuteOrderResponse()
            {
                OrderId = openResponse?.OrderId,
                ErrorMessage = openResponse?.ErrorMessage,
            };
        }

        // Здесь пока игнорим количество лотов в сигнале и используем только знак для реверса позиции.
        // Более продвинутый вариант - аджастить поцизию под количество, запрошенное в сигнале.

        if (Math.Sign(currentLots) == Math.Sign(request.QtyLots))
        {
            return new ExecuteOrderResponse
            {
                ErrorMessage = "Trading signal and position have the same direction."
            };
        }

        var reverseRequest = new ReversePositionCommand
        {
            RequestId = request.RequestId,
            PortfolioIdentity = request.PortfolioIdentity,
            InstrumentId = request.InstrumentId,
        };

        var reverseResponse = await Mediator.Send(reverseRequest, cancellationToken);

        return new ExecuteOrderResponse()
        {
            OrderId = reverseResponse?.OrderId,
            ErrorMessage = reverseResponse?.ErrorMessage,
        };
    }
}
