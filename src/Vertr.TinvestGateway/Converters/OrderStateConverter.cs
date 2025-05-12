using Vertr.TinvestGateway.Contracts;

namespace Vertr.TinvestGateway.Converters;

internal static class OrderStateConverter
{
    public static OrderState Convert(this Tinkoff.InvestApi.V1.OrderState source)
        => new OrderState
        {
            OrderId = source.OrderId,
            AveragePositionPrice = source.AveragePositionPrice,
            Currency = source.Currency,
            Direction = source.Direction.Convert(),
            ExecutedCommission = source.ExecutedCommission,
            ExecutedOrderPrice = source.ExecutedOrderPrice,
            ExecutionReportStatus = source.ExecutionReportStatus.Convert(),
            InitialCommission = source.InitialCommission,
            InitialOrderPrice = source.InitialOrderPrice,
            InitialSecurityPrice = source.InitialSecurityPrice,
            InstrumentUid = source.InstrumentUid,
            LotsExecuted = source.LotsExecuted,
            LotsRequested = source.LotsRequested,
            OrderDate = source.OrderDate.ToDateTime(),
            OrderRequestId = source.OrderRequestId,
            OrderType = source.OrderType.Convert(),
            ServiceCommission = source.ServiceCommission,
            TotalOrderAmount = source.TotalOrderAmount,
            OrderStages = source.Stages.ToArray().Convert(),
        };

    public static Trade Convert(this Tinkoff.InvestApi.V1.OrderStage source)
        => new Trade
        {
            ExecutionTime = source.ExecutionTime.ToDateTime(),
            Price = source.Price,
            Quantity = source.Quantity,
            TradeId = source.TradeId,
        };

    public static Trade[] Convert(this Tinkoff.InvestApi.V1.OrderStage[] source)
        => [.. source.Select(t => t.Convert())];
}
