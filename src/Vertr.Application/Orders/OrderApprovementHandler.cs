using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vertr.Adapters.Tinvest;
using Vertr.Domain;
using Vertr.Domain.Enums;
using Vertr.Domain.Repositories;

namespace Vertr.Application.Orders;

internal class OrderApprovementHandler : IRequestHandler<OrderApprovementRequest, OrderApprovementResponse>
{
    private readonly ITinvestPortfolioRepository _portfolioRepository;
    private readonly TinvestSettings _settings;
    private readonly ILogger<OrderApprovementHandler> _logger;

    private static readonly OrderApprovementResponse _empty = new OrderApprovementResponse
    {
        ApprovedQuantityLots = 0,
        OrderDirection = OrderDirection.Unspecified,
    };

    public OrderApprovementHandler(
        ITinvestPortfolioRepository portfolioRepository,
        IOptions<TinvestSettings> options,
        ILogger<OrderApprovementHandler> logger)
    {
        _portfolioRepository = portfolioRepository;
        _settings = options.Value;
        _logger = logger;
    }

    public async Task<OrderApprovementResponse> Handle(
        OrderApprovementRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Signal.Action == TradeAction.Hold)
        {
            return _empty;
        }

        var lastPortfoliosnapshot = await _portfolioRepository.GetLast(request.AccountId);

        if (lastPortfoliosnapshot == null)
        {
            _logger.LogError($"Cannot find portfolio snapshot for accountId={request.AccountId}");
            return _empty;
        }

        var position = GetPosition(lastPortfoliosnapshot, request.Signal.Symbol);

        if (position == null || position.Quantity == 0)
        {
            // Open New position
            var response = new OrderApprovementResponse
            {
                ApprovedQuantityLots = request.Signal.QuantityLots,
                OrderDirection = GetDirectionByAction(request.Signal.Action)
            };

            _logger.LogInformation($"New position approved for accountId={request.AccountId}. Direction={response.OrderDirection}");

            return response;
        }

        // Позиция в штуках, ордер - в лотах
        // Позиция - шорт - отрицательная, лонг - положительная
        var positionQty = position.Quantity;

        // Current position - Long
        if (positionQty > 0)
        {
            if (request.Signal.Action == TradeAction.Buy)
            {
                // Skip
                return _empty;
            }
            else
            {
                // Revert positions from Long to Short
                var response = new OrderApprovementResponse
                {
                    ApprovedQuantityLots = request.Signal.QuantityLots * 2,
                    OrderDirection = OrderDirection.Sell,
                };
            }
        }
        else // Current position - Short
        {
            if (request.Signal.Action == TradeAction.Sell)
            {
                // Skip
                return _empty;
            }
            else
            {
                // Revert positions from Short to Long
                var response = new OrderApprovementResponse
                {
                    ApprovedQuantityLots = request.Signal.QuantityLots * 2,
                    OrderDirection = OrderDirection.Buy,
                };
            }
        }

        return _empty;
    }

    private PortfolioPosition? GetPosition(PortfolioSnapshot snapshot, string symbol)
    {
        var symbolId = _settings.GetSymbolId(symbol);

        if (string.IsNullOrEmpty(symbolId))
        {
            return null;
        }

        var instrumentId = Guid.Parse(symbolId);
        return snapshot.Positions.FirstOrDefault(p => p.InstrumentUid == instrumentId);
    }

    private OrderDirection GetDirectionByAction(TradeAction action)
        => action switch
        {
            TradeAction.Hold => OrderDirection.Unspecified,
            TradeAction.Sell => OrderDirection.Sell,
            TradeAction.Buy => OrderDirection.Buy,
            _ => OrderDirection.Unspecified,
        };
}
