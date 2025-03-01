using AutoMapper;
using Vertr.Domain;
using Vertr.Domain.Enums;

namespace Vertr.Adapters.Tinvest.Converters;
internal static class PostOrderResponseConverter
{
    public static PostOrderResponse Convert(
        this Tinkoff.InvestApi.V1.PostOrderResponse response,
        string accountId,
        IMapper mapper) => new PostOrderResponse
        {
            Id = Guid.NewGuid(),
            TimeUtc = DateTime.UtcNow,
            AccountId = accountId,
            OrderId = response.OrderId,
            OrderRequestId = response.OrderRequestId,
            ExecutionReportStatus = mapper.Map<OrderExecutionReportStatus>(response.ExecutionReportStatus),
            LotsRequested = response.LotsRequested,
            LotsExecuted = response.LotsExecuted,
            InitialOrderPrice = response.InitialOrderPrice,
            ExecutedOrderPrice = response.ExecutedOrderPrice,
            TotalOrderAmount = response.TotalOrderAmount,
            InitialCommission = response.InitialCommission,
            ExecutedCommission = response.ExecutedCommission,
            Direction = mapper.Map<OrderDirection>(response.Direction),
            InitialSecurityPrice = response.InitialSecurityPrice,
            OrderType = mapper.Map<OrderType>(response.OrderType),
            Message = response.Message,
            InstrumentUid = Guid.Parse(response.InstrumentUid),
        };
}
