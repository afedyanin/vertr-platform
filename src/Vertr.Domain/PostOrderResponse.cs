using Vertr.Domain.Enums;

namespace Vertr.Domain;
public record class PostOrderResponse
{
    public Guid Id { get; set; }

    public Guid? TradingSignalId { get; set; }

    public DateTime TimeUtc { get; set; }

    public required string AccountId { get; set; }

    public string OrderId { get; set; } = string.Empty;

    public string OrderRequestId { get; set; } = string.Empty;

    public OrderExecutionReportStatus ExecutionReportStatus { get; set; }

    public long LotsRequested { get; set; }

    public long LotsExecuted { get; set; }

    public decimal InitialOrderPrice { get; set; }

    public decimal ExecutedOrderPrice { get; set; }

    public decimal TotalOrderAmount { get; set; }

    public decimal InitialCommission { get; set; }

    public decimal ExecutedCommission { get; set; }

    public OrderDirection Direction { get; set; }

    public decimal InitialSecurityPrice { get; set; }

    public OrderType OrderType { get; set; }

    public string Message { get; set; } = string.Empty;

    public Guid InstrumentUid { get; set; }
}
