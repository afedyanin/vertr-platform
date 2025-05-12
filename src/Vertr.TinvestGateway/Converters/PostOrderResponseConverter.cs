using Vertr.TinvestGateway.Contracts;

namespace Vertr.TinvestGateway.Converters;
internal static class PostOrderResponseConverter
{
    public static PostOrderResponse Convert(
        this Tinkoff.InvestApi.V1.PostOrderResponse response)
        => new PostOrderResponse
        {
            OrderId = response.OrderId,
            OrderRequestId = response.OrderRequestId,
            ExecutionReportStatus = response.ExecutionReportStatus.Convert(),
            LotsRequested = response.LotsRequested,
            LotsExecuted = response.LotsExecuted,
            InitialOrderPrice = response.InitialOrderPrice,
            ExecutedOrderPrice = response.ExecutedOrderPrice,
            TotalOrderAmount = response.TotalOrderAmount,
            InitialCommission = response.InitialCommission,
            ExecutedCommission = response.ExecutedCommission,
            InitialSecurityPrice = response.InitialSecurityPrice,
            Message = response.Message,
            InstrumentUid = response.InstrumentUid,
            OrderType = response.OrderType.Convert(),
            OrderDirection = response.Direction.Convert(),
        };
}
