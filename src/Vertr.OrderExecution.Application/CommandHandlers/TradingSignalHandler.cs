using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Commands;
using Vertr.Platform.Common;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.OrderExecution.Application.CommandHandlers;

internal class TradingSignalHandler : OrderHandlerBase, ICommandHandler<TradingSignalCommand, ExecuteOrderResponse>
{
    private readonly ICommandHandler<OpenPositionCommand, ExecuteOrderResponse> _openPositionHandler;
    private readonly ICommandHandler<ReversePositionCommand, ExecuteOrderResponse> _reversePositionHandler;

    public TradingSignalHandler(
        ICommandHandler<OpenPositionCommand, ExecuteOrderResponse> openPositionHandler,
        ICommandHandler<ReversePositionCommand, ExecuteOrderResponse> reversePositionHandler,
        IPortfolioRepository portfolioRepository,
        IStaticMarketDataProvider staticMarketDataProvider
        ) : base(portfolioRepository, staticMarketDataProvider)
    {
        _openPositionHandler = openPositionHandler;
        _reversePositionHandler = reversePositionHandler;
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

            var openResponse = await _openPositionHandler.Handle(openRequest, cancellationToken);

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

        var reverseResponse = await _reversePositionHandler.Handle(reverseRequest, cancellationToken);

        return new ExecuteOrderResponse()
        {
            OrderId = reverseResponse?.OrderId,
            ErrorMessage = reverseResponse?.ErrorMessage,
        };
    }
}
