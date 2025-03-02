using AutoMapper;
using Vertr.Domain;

namespace Vertr.Adapters.Tinvest.Converters;
internal static class PostOrderRequestConverter
{
    public static Tinkoff.InvestApi.V1.PostOrderRequest Convert(
        this PostOrderRequest request,
        TinvestSettings tinvestSettings,
        IMapper mapper) => new Tinkoff.InvestApi.V1.PostOrderRequest
        {
            AccountId = request.AccountId,
            Direction = mapper.Map<Tinkoff.InvestApi.V1.OrderDirection>(request.OrderDirection),
            OrderType = mapper.Map<Tinkoff.InvestApi.V1.OrderType>(request.OrderType),
            Price = request.Price,
            PriceType = mapper.Map<Tinkoff.InvestApi.V1.PriceType>(request.PriceType),
            Quantity = request.QuantityLots,
            OrderId = request.RequestId.ToString(),
            TimeInForce = mapper.Map<Tinkoff.InvestApi.V1.TimeInForceType>(request.TimeInForceType),
            InstrumentId = tinvestSettings.GetSymbolId(request.Symbol),
        };
}
