using Vertr.OrderExecution.Contracts;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.TinvestGateway.Application.Converters;

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
            InstrumentId = Guid.Parse(source.InstrumentUid),
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
            Price = source.Price.Convert(),
            Quantity = source.Quantity,
            TradeId = source.TradeId,
        };

    public static Trade[] Convert(this Tinkoff.InvestApi.V1.OrderStage[] source)
        => [.. source.Select(t => t.Convert())];

    public static Trade Convert(this Tinkoff.InvestApi.V1.OrderTrade source)
        => new Trade
        {
            ExecutionTime = source.DateTime.ToDateTime(),
            Price = new Money(source.Price),
            Quantity = source.Quantity,
            TradeId = source.TradeId,
        };

    public static Trade[] Convert(
        this Tinkoff.InvestApi.V1.OrderTrade[] source)
        => [.. source.Select(t => t.Convert())];

    public static OrderState Convert(this Tinkoff.InvestApi.V1.OrderStateStreamResponse.Types.OrderState source)
        => new OrderState
        {
            OrderId = source.OrderId,
            OrderRequestId = source.OrderRequestId,
            OrderDate = source.CreatedAt?.ToDateTime(),
            ExecutionReportStatus = source.ExecutionReportStatus.Convert(),
            InstrumentId = Guid.Parse(source.InstrumentUid),
            Direction = source.Direction.Convert(),
            OrderType = source.OrderType.Convert(),
            InitialOrderPrice = source.InitialOrderPrice,
            ExecutedOrderPrice = source.ExecutedOrderPrice,
            Currency = source.Currency,
            LotsRequested = source.LotsRequested,
            LotsExecuted = source.LotsExecuted,
            OrderStages = source.Trades.ToArray().Convert(),
        };
}
