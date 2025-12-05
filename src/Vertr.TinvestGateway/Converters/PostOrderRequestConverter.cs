using Vertr.TinvestGateway.Contracts.Orders;

namespace Vertr.TinvestGateway.Converters;

internal static class PostOrderRequestConverter
{
    public static Tinkoff.InvestApi.V1.PostOrderRequest Convert(this PostOrderRequest request, string accountId)
            => new Tinkoff.InvestApi.V1.PostOrderRequest
            {
                AccountId = accountId,
                Direction = request.OrderDirection.Convert(),
                OrderType = request.OrderType.Convert(),
                Price = request.Price,
                PriceType = request.PriceType.Convert(),
                Quantity = request.QuantityLots,
                OrderId = request.RequestId.ToString(),
                TimeInForce = request.TimeInForceType.Convert(),
                InstrumentId = request.InstrumentId.ToString(),
            };
}