using MediatR;
using Vertr.OrderExecution.Application.Abstractions;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.OrderExecution.Application.Commands;

internal class TradingSignalHandler : PositionHandlerBase, IRequestHandler<TradingSignalRequest, OrderExecutionResponse>
{
    public TradingSignalHandler(
        IMediator mediator,
        IPortfolioClient portfolioClient,
        IStaticMarketDataProvider staticMarketDataProvider
        ) : base(mediator, portfolioClient, staticMarketDataProvider)
    {
    }

    public async Task<OrderExecutionResponse> Handle(
        TradingSignalRequest request,
        CancellationToken cancellationToken)
    {
        if (request.QtyLots == 0L)
        {
            return new OrderExecutionResponse
            {
                ErrorMessage = "Trading signal quantity is empty."
            };
        }

        var currentLots = await GetCurrentPositionInLots(request.PortfolioId, request.InstrumentId);

        if (currentLots == 0L)
        {
            var openRequest = new OpenPositionRequest
            {
                PortfolioId = request.PortfolioId,
                InstrumentId = request.InstrumentId,
                RequestId = request.RequestId,
                QtyLots = request.QtyLots,
            };

            var openResponse = await Mediator.Send(openRequest, cancellationToken);

            return new OrderExecutionResponse()
            {
                OrderId = openResponse?.OrderId,
                ErrorMessage = openResponse?.ErrorMessage,
            };
        }

        // Здесь пока игнорим количество лотов в сигнале и используем только знак для реверса позиции.
        // Более продвинутый вариант - аджастить поцизию под количество, запрошенное в сигнале.

        if (Math.Sign(currentLots) == Math.Sign(request.QtyLots))
        {
            return new OrderExecutionResponse
            {
                ErrorMessage = "Trading signal and position have the same direction."
            };
        }

        var reverseRequest = new ReversePositionRequest
        {
            PortfolioId = request.PortfolioId,
            InstrumentId = request.InstrumentId,
            RequestId = request.RequestId,
        };

        var reverseResponse = await Mediator.Send(reverseRequest, cancellationToken);

        return new OrderExecutionResponse()
        {
            OrderId = reverseResponse?.OrderId,
            ErrorMessage = reverseResponse?.ErrorMessage,
        };
    }
}
