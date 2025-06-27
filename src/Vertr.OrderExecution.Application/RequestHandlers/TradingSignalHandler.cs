using MediatR;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.OrderExecution.Application.RequestHandlers;

internal class TradingSignalHandler : PositionHandlerBase, IRequestHandler<TradingSignalRequest, ExecuteOrderResponse>
{
    public TradingSignalHandler(
        IMediator mediator,
        IPortfolioRepository portfolioRepository,
        IStaticMarketDataProvider staticMarketDataProvider
        ) : base(mediator, portfolioRepository, staticMarketDataProvider)
    {
    }

    public async Task<ExecuteOrderResponse> Handle(
        TradingSignalRequest request,
        CancellationToken cancellationToken)
    {
        if (request.QtyLots == 0L)
        {
            return new ExecuteOrderResponse
            {
                ErrorMessage = "Trading signal quantity is empty."
            };
        }

        var currentLots = await GetCurrentPositionInLots(request.PortfolioIdentity, request.InstrumentIdentity);

        if (currentLots == 0L)
        {
            var openRequest = new OpenPositionRequest
            {
                RequestId = request.RequestId,
                PortfolioIdentity = request.PortfolioIdentity,
                InstrumentIdentity = request.InstrumentIdentity,
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

        var reverseRequest = new ReversePositionRequest
        {
            RequestId = request.RequestId,
            PortfolioIdentity = request.PortfolioIdentity,
            InstrumentIdentity = request.InstrumentIdentity,
        };

        var reverseResponse = await Mediator.Send(reverseRequest, cancellationToken);

        return new ExecuteOrderResponse()
        {
            OrderId = reverseResponse?.OrderId,
            ErrorMessage = reverseResponse?.ErrorMessage,
        };
    }
}
