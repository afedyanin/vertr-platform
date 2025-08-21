using Microsoft.Extensions.Options;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Commands;
using Vertr.Platform.Common.Mediator;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.OrderExecution.Application.CommandHandlers;

internal class TradingSignalHandler : OrderHandlerBase, IRequestHandler<TradingSignalRequest, ExecuteOrderResponse>
{
    public TradingSignalHandler(
        IMediator mediator,
        IPortfolioProvider portfolioProvider,
        IInstrumentsRepository staticMarketDataProvider,
        IOptions<OrderExecutionSettings> options) :
        base(mediator, portfolioProvider, staticMarketDataProvider, options)
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

        var currentLots = await GetCurrentPositionInLots(request.PortfolioId, request.InstrumentId);

        if (currentLots == 0L)
        {
            var openRequest = new OpenPositionRequest
            {
                RequestId = request.RequestId,
                PortfolioId = request.PortfolioId,
                InstrumentId = request.InstrumentId,
                QtyLots = request.QtyLots,
                CreatedAt = request.CreatedAt,
                Price = request.Price,
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
            PortfolioId = request.PortfolioId,
            InstrumentId = request.InstrumentId,
            CreatedAt = request.CreatedAt,
            Price = request.Price
        };

        var reverseResponse = await Mediator.Send(reverseRequest, cancellationToken);

        return new ExecuteOrderResponse()
        {
            OrderId = reverseResponse?.OrderId,
            ErrorMessage = reverseResponse?.ErrorMessage,
        };
    }
}
