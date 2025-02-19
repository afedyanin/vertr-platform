namespace Vertr.Adapters.Tinvest.Converters;
internal static class OrderExecutionStatusConverter
{
    public static Domain.OrderExecutionStatus Convert(this Tinkoff.InvestApi.V1.OrderExecutionReportStatus status)
        => status switch
        {
            Tinkoff.InvestApi.V1.OrderExecutionReportStatus.ExecutionReportStatusUnspecified => Domain.OrderExecutionStatus.Unspecified,
            Tinkoff.InvestApi.V1.OrderExecutionReportStatus.ExecutionReportStatusFill => Domain.OrderExecutionStatus.Fill,
            Tinkoff.InvestApi.V1.OrderExecutionReportStatus.ExecutionReportStatusNew => Domain.OrderExecutionStatus.New,
            Tinkoff.InvestApi.V1.OrderExecutionReportStatus.ExecutionReportStatusCancelled => Domain.OrderExecutionStatus.Cancelled,
            Tinkoff.InvestApi.V1.OrderExecutionReportStatus.ExecutionReportStatusRejected => Domain.OrderExecutionStatus.Rejected,
            Tinkoff.InvestApi.V1.OrderExecutionReportStatus.ExecutionReportStatusPartiallyfill => Domain.OrderExecutionStatus.Partiallyfill,
            _ => throw new InvalidOperationException($"Unknown OrderExecutionReportStatus={status}"),
        };
}
