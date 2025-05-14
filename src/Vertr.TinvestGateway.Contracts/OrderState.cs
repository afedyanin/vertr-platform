namespace Vertr.TinvestGateway.Contracts;

public record class OrderState
{
    public string OrderId { get; init; } = string.Empty;

    public OrderExecutionReportStatus ExecutionReportStatus { get; init; }

    public long LotsRequested { get; init; }

    public long LotsExecuted { get; init; }

    public decimal InitialOrderPrice { get; init; }

    public decimal ExecutedOrderPrice { get; init; }

    public decimal TotalOrderAmount { get; init; }

    public decimal AveragePositionPrice { get; init; }

    public decimal InitialCommission { get; init; }

    public decimal ExecutedCommission { get; init; }

    public OrderDirection Direction { get; init; }

    public decimal InitialSecurityPrice { get; init; }

    public decimal ServiceCommission { get; init; }

    public string Currency { get; init; } = string.Empty;

    public OrderType OrderType { get; init; }

    public DateTime? OrderDate { get; init; }

    public string InstrumentId { get; init; } = string.Empty;

    public string OrderRequestId { get; init; } = string.Empty;

    public Trade[] OrderStages { get; init; } = [];
}
