using Vertr.OrderExecution.Application.Abstractions;
using Vertr.OrderExecution.Application.Entities;
using Vertr.TinvestGateway.Contracts;

namespace Vertr.OrderExecution.Application.Services;

internal class TinvestOrderExecutionService : IOrderExecutionService
{
    private readonly ITinvestGateway _tinvestGateway;

    public TinvestOrderExecutionService(
        ITinvestGateway tinvestGateway
        )
    {
        _tinvestGateway = tinvestGateway;
    }

    public async Task<PostOrderResult> PostMarketOrder(
        Guid requestId,
        Guid instrumentId,
        string accountId,
        long qtyLots)
    {
        var request = new PostOrderRequest
        {
            AccountId = accountId,
            RequestId = requestId,
            InstrumentId = instrumentId,
            OrderDirection = qtyLots > 0 ? OrderDirection.Buy : OrderDirection.Sell,
            Price = decimal.Zero,
            OrderType = OrderType.Market,
            PriceType = PriceType.Unspecified,
            TimeInForceType = TimeInForceType.Unspecified,
            QuantityLots = Math.Abs(qtyLots),
        };

        var response = await _tinvestGateway.PostOrder(request);

        return new PostOrderResult();
    }
}
