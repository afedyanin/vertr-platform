using Vertr.TinvestGateway.Contracts.Orders;

namespace Vertr.TinvestGateway.Converters;

public static class OrderStateConverter
{
    public static OrderState Convert(this Tinkoff.InvestApi.V1.OrderState source)
        => new OrderState
        {
            Id = Guid.NewGuid(),
            OrderId = source.OrderId,
            AveragePositionPrice = source.AveragePositionPrice.Convert(),
            Currency = source.Currency,
            Direction = source.Direction.Convert(),
            ExecutedCommission = source.ExecutedCommission.Convert(),
            ExecutedOrderPrice = source.ExecutedOrderPrice.Convert(),
            ExecutionReportStatus = source.ExecutionReportStatus.Convert(),
            InitialCommission = source.InitialCommission.Convert(),
            InitialOrderPrice = source.InitialOrderPrice.Convert(),
            InitialSecurityPrice = source.InitialSecurityPrice.Convert(),
            InstrumentId = Guid.Parse(source.InstrumentUid),
            LotsExecuted = source.LotsExecuted,
            LotsRequested = source.LotsRequested,
            CreatedAt = source.OrderDate.ToDateTime(),
            OrderRequestId = source.OrderRequestId,
            OrderType = source.OrderType.Convert(),
            ServiceCommission = source.ServiceCommission.Convert(),
            TotalOrderAmount = source.TotalOrderAmount.Convert(),
            OrderStages = source.Stages.ToArray().Convert(),
        };

    public static OrderState Convert(
        this Tinkoff.InvestApi.V1.OrderStateStreamResponse.Types.OrderState source,
        string accountId)
        => new OrderState
        {
            Id = Guid.NewGuid(),
            OrderId = source.OrderId,
            OrderRequestId = source.OrderRequestId,
            CreatedAt = source.CreatedAt?.ToDateTime() ?? DateTime.UtcNow,
            ExecutionReportStatus = source.ExecutionReportStatus.Convert(),
            InstrumentId = Guid.Parse(source.InstrumentUid),
            Direction = source.Direction.Convert(),
            OrderType = source.OrderType.Convert(),
            InitialOrderPrice = source.InitialOrderPrice.Convert(),
            ExecutedOrderPrice = source.ExecutedOrderPrice.Convert(),
            Currency = source.Currency,
            LotsRequested = source.LotsRequested,
            LotsExecuted = source.LotsExecuted,
            OrderStages = source.Trades?.ToArray().Convert(source.Currency) ?? [],
            AccountId = accountId
        };

    public static Trade[] Convert(this Tinkoff.InvestApi.V1.OrderStage[] source)
        => [.. source.Select(t => t.Convert())];

}