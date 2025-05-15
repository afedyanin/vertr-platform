using MediatR;
using Vertr.OrderExecution.Application.Abstractions;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.OrderExecution.Application.Commands;

internal class TradingSignalHandler : OrderHandlerBase, IRequestHandler<TradingSignalRequest, TradingSignalResponse>
{
    public TradingSignalHandler(
        IMediator mediator,
        IPortfolioClient portfolioClient,
        IStaticMarketDataProvider staticMarketDataProvider
        ) : base(mediator, portfolioClient, staticMarketDataProvider)
    {
    }

    public async Task<TradingSignalResponse> Handle(
        TradingSignalRequest request,
        CancellationToken cancellationToken)
    {
        if (request.QtyLots == 0L)
        {
            return new TradingSignalResponse
            {
                Message = "Trading Signal Qty is empty."
            };
        }

        var currentLots = await GetCurrentPositionInLots(request.AccountId, request.InstrumentId);

        if (currentLots == 0L)
        {
            var openRequest = new OpenPositionRequest
            {
                AccountId = request.AccountId,
                InstrumentId = request.InstrumentId,
                RequestId = request.RequestId,
                QtyLots = request.QtyLots,
            };

            var openResponse = await Mediator.Send(openRequest, cancellationToken);

            return new TradingSignalResponse()
            {
                PostOrderResult = openResponse?.PostOrderResult,
                Message = openResponse?.Message,
            };
        }

        // Здесь пока игнорим количество лотов в сигнале и используем только знак для реверса позиции.
        // Более продвинутый вариант - аджасить поцизию под количество, запрошенное в сигнале.

        if (Math.Sign(currentLots) == Math.Sign(request.QtyLots))
        {
            return new TradingSignalResponse
            {
                Message = "Trading signal and position have the same direction."
            };
        }

        var reverseRequest = new ReversePositionRequest
        {
            AccountId = request.AccountId,
            InstrumentId = request.InstrumentId,
            RequestId = request.RequestId,
        };

        var reverseResponse = await Mediator.Send(reverseRequest, cancellationToken);

        return new TradingSignalResponse()
        {
            PostOrderResult = reverseResponse?.PostOrderResult,
            Message = reverseResponse?.Message,
        };
    }
}
