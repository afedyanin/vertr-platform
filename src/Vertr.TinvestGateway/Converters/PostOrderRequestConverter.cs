using Vertr.TinvestGateway.Contracts;

namespace Vertr.TinvestGateway.Converters;
internal static class PostOrderRequestConverter
{
    public static Tinkoff.InvestApi.V1.PostOrderRequest Convert(
        this PostOrderRequest request,
        TinvestSettings tinvestSettings)
            => new Tinkoff.InvestApi.V1.PostOrderRequest
            {
                AccountId = request.AccountId,
                Direction = request.OrderDirection.Convert(),
                OrderType = request.OrderType.Convert(),
                Price = request.Price,
                PriceType = request.PriceType.Convert(),
                Quantity = request.QuantityLots,
                OrderId = request.RequestId.ToString(),
                TimeInForce = request.TimeInForceType.Convert(),
                InstrumentId = tinvestSettings.GetSymbolId(request.Symbol),
            };
}
