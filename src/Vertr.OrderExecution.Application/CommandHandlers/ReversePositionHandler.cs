using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Commands;
using Vertr.Platform.Common.Mediator;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.OrderExecution.Application.CommandHandlers;

internal class ReversePositionHandler : OrderHandlerBase, IRequestHandler<ReversePositionRequest, ExecuteOrderResponse>
{
    private readonly ILogger<ReversePositionHandler> _logger;
    public ReversePositionHandler(
        IMediator mediator,
        IPortfolioRepository portfolioProvider,
        IInstrumentsRepository staticMarketDataProvider,
        IOptions<OrderExecutionSettings> options,
        ILogger<ReversePositionHandler> logger) :
        base(mediator, portfolioProvider, staticMarketDataProvider, options)
    {
        _logger = logger;
    }

    public async Task<ExecuteOrderResponse> Handle(
        ReversePositionRequest request,
        CancellationToken cancellationToken)
    {
        var currentLots = await GetCurrentPositionInLots(request.PortfolioId, request.InstrumentId);

        if (currentLots == 0L)
        {
            return new ExecuteOrderResponse()
            {
                ErrorMessage = "Position closed."
            };
        }

        var lotsToRevert = currentLots * -2L;

        // TODO: Remove me
        _logger.LogDebug($"CurrentLots={currentLots} LotsToRevert={lotsToRevert}");

        // TODO: Remove me
        if (Math.Abs(lotsToRevert) > 100)
        {
            throw new InvalidOperationException($"Lot size maximum exceeded. CurrentLots={currentLots} LotsToRevert={lotsToRevert}");
        }

        var orderRequest = new ExecuteOrderRequest
        {
            RequestId = request.RequestId,
            PortfolioId = request.PortfolioId,
            InstrumentId = request.InstrumentId,
            QtyLots = lotsToRevert,
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
