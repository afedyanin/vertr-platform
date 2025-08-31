using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Commands;
using Vertr.Platform.Common.Mediator;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.OrderExecution.Application.CommandHandlers;

internal class TradingSignalHandler : OrderHandlerBase, IRequestHandler<TradingSignalRequest, ExecuteOrderResponse>
{
    private readonly ILogger<TradingSignalHandler> _logger;
    private readonly IPortfolioAwatingService _portfolioAwatingService;

    public TradingSignalHandler(
        IMediator mediator,
        IPortfolioRepository portfolioProvider,
        IInstrumentsRepository staticMarketDataProvider,
        IPortfolioAwatingService portfolioAwatingService,
        IOptions<OrderExecutionSettings> options,
        ILogger<TradingSignalHandler> logger) :
        base(mediator, portfolioProvider, staticMarketDataProvider, options)
    {
        _logger = logger;
        _portfolioAwatingService = portfolioAwatingService;
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

        // Нужно синхронизировать обрабтку сингалов с обновлением портфеля
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

            // TODO: Refactor this
            if (request.BacktestId.HasValue)
            {
                _logger.LogDebug($"Open position: Sart awaitng process portfolio: Id={request.PortfolioId}");
                await _portfolioAwatingService.WaitToComplete(request.PortfolioId, cancellationToken);
            }

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

        // TODO: Refactor this
        if (request.BacktestId.HasValue)
        {
            _logger.LogDebug($"Reverse position: Sart awaitng process portfolio: Id={request.PortfolioId}");
            await _portfolioAwatingService.WaitToComplete(request.PortfolioId, cancellationToken);
        }

        return new ExecuteOrderResponse()
        {
            OrderId = reverseResponse?.OrderId,
            ErrorMessage = reverseResponse?.ErrorMessage,
        };
    }
}
