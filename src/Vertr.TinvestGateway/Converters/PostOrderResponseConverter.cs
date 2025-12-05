using Vertr.TinvestGateway.Contracts.Orders;

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
            InitialOrderPrice = response.InitialOrderPrice.Convert(),
            ExecutedOrderPrice = response.ExecutedOrderPrice.Convert(),
            TotalOrderAmount = response.TotalOrderAmount.Convert(),
            InitialCommission = response.InitialCommission.Convert(),
            ExecutedCommission = response.ExecutedCommission.Convert(),
            InitialSecurityPrice = response.InitialSecurityPrice.Convert(),
            Message = response.Message,
            InstrumentId = Guid.Parse(response.InstrumentUid),
            OrderType = response.OrderType.Convert(),
            OrderDirection = response.Direction.Convert(),
        };
}