namespace Vertr.TinvestGateway.Contracts;
public record class PostOrderResponse
{
    public string OrderId { get; init; } = string.Empty;

    public string OrderRequestId { get; init; } = string.Empty;

    public OrderExecutionReportStatus ExecutionReportStatus { get; init; }

    public OrderType OrderType { get; init; }

    public OrderDirection OrderDirection { get; init; }

    public long LotsRequested { get; init; }

    public long LotsExecuted { get; init; }

    public decimal InitialOrderPrice { get; init; }

    public decimal ExecutedOrderPrice { get; init; }

    public decimal TotalOrderAmount { get; init; }

    public decimal InitialCommission { get; init; }

    public decimal ExecutedCommission { get; init; }

    public decimal InitialSecurityPrice { get; init; }

    public string Message { get; init; } = string.Empty;

    public string InstrumentUid { get; init; } = string.Empty;
}
