using Vertr.TinvestGateway.Contracts.Orders.Enums;

namespace Vertr.TinvestGateway.Converters;

internal static class OrderEnumsConverter
{
    public static Tinkoff.InvestApi.V1.OrderType Convert(this OrderType orderType)
        => orderType switch
        {
            OrderType.Unspecified => Tinkoff.InvestApi.V1.OrderType.Unspecified,
            OrderType.Limit => Tinkoff.InvestApi.V1.OrderType.Limit,
            OrderType.Market => Tinkoff.InvestApi.V1.OrderType.Market,
            OrderType.Bestprice => Tinkoff.InvestApi.V1.OrderType.Bestprice,
            _ => throw new NotImplementedException(),
        };

    public static OrderType Convert(this Tinkoff.InvestApi.V1.OrderType orderType)
        => orderType switch
        {
            Tinkoff.InvestApi.V1.OrderType.Unspecified => OrderType.Unspecified,
            Tinkoff.InvestApi.V1.OrderType.Limit => OrderType.Limit,
            Tinkoff.InvestApi.V1.OrderType.Market => OrderType.Market,
            Tinkoff.InvestApi.V1.OrderType.Bestprice => OrderType.Bestprice,
            _ => throw new NotImplementedException(),
        };

    public static Tinkoff.InvestApi.V1.OrderDirection Convert(this OrderDirection orderDirection)
        => orderDirection switch
        {
            OrderDirection.Unspecified => Tinkoff.InvestApi.V1.OrderDirection.Unspecified,
            OrderDirection.Buy => Tinkoff.InvestApi.V1.OrderDirection.Buy,
            OrderDirection.Sell => Tinkoff.InvestApi.V1.OrderDirection.Sell,
            _ => throw new NotImplementedException(),
        };

    public static OrderDirection Convert(this Tinkoff.InvestApi.V1.OrderDirection orderDirection)
        => orderDirection switch
        {
            Tinkoff.InvestApi.V1.OrderDirection.Unspecified => OrderDirection.Unspecified,
            Tinkoff.InvestApi.V1.OrderDirection.Buy => OrderDirection.Buy,
            Tinkoff.InvestApi.V1.OrderDirection.Sell => OrderDirection.Sell,
            _ => throw new NotImplementedException(),
        };

    public static Tinkoff.InvestApi.V1.TimeInForceType Convert(this TimeInForceType source)
        => source switch
        {
            TimeInForceType.Unspecified => Tinkoff.InvestApi.V1.TimeInForceType.TimeInForceUnspecified,
            TimeInForceType.FillAndKill => Tinkoff.InvestApi.V1.TimeInForceType.TimeInForceFillAndKill,
            TimeInForceType.FillOrKill => Tinkoff.InvestApi.V1.TimeInForceType.TimeInForceFillOrKill,
            TimeInForceType.Day => Tinkoff.InvestApi.V1.TimeInForceType.TimeInForceDay,
            _ => throw new NotImplementedException(),
        };

    public static Tinkoff.InvestApi.V1.PriceType Convert(this PriceType source)
        => source switch
        {
            PriceType.Unspecified => Tinkoff.InvestApi.V1.PriceType.Unspecified,
            PriceType.Currency => Tinkoff.InvestApi.V1.PriceType.Currency,
            PriceType.Point => Tinkoff.InvestApi.V1.PriceType.Point,
            _ => throw new NotImplementedException(),
        };

    public static OrderExecutionReportStatus Convert(this Tinkoff.InvestApi.V1.OrderExecutionReportStatus source)
        => source switch
        {
            Tinkoff.InvestApi.V1.OrderExecutionReportStatus.ExecutionReportStatusUnspecified => OrderExecutionReportStatus.Unspecified,
            Tinkoff.InvestApi.V1.OrderExecutionReportStatus.ExecutionReportStatusNew => OrderExecutionReportStatus.New,
            Tinkoff.InvestApi.V1.OrderExecutionReportStatus.ExecutionReportStatusFill => OrderExecutionReportStatus.Fill,
            Tinkoff.InvestApi.V1.OrderExecutionReportStatus.ExecutionReportStatusRejected => OrderExecutionReportStatus.Rejected,
            Tinkoff.InvestApi.V1.OrderExecutionReportStatus.ExecutionReportStatusCancelled => OrderExecutionReportStatus.Cancelled,
            Tinkoff.InvestApi.V1.OrderExecutionReportStatus.ExecutionReportStatusPartiallyfill => OrderExecutionReportStatus.Partiallyfill,
            _ => throw new NotImplementedException(),
        };
}