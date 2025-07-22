using Vertr.OrderExecution.Contracts.Enums;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.OrderExecution.Contracts;

public record class OrderState
{
    public string OrderId { get; init; } = string.Empty;

    public OrderExecutionReportStatus ExecutionReportStatus { get; init; }

    public long LotsRequested { get; init; }

    public long LotsExecuted { get; init; }

    public Money? InitialOrderPrice { get; init; }

    public Money? ExecutedOrderPrice { get; init; }

    public Money? TotalOrderAmount { get; init; }

    public Money? AveragePositionPrice { get; init; }

    public Money? InitialCommission { get; init; }

    public Money? ExecutedCommission { get; init; }

    public OrderDirection Direction { get; init; }

    public Money? InitialSecurityPrice { get; init; }

    public Money? ServiceCommission { get; init; }

    public string Currency { get; init; } = string.Empty;

    public OrderType OrderType { get; init; }

    public DateTime? OrderDate { get; init; }

    public Guid InstrumentId { get; init; }

    public string OrderRequestId { get; init; } = string.Empty;

    public Trade[] OrderStages { get; init; } = [];

    public string? AccountId { get; init; }
}
