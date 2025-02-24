using Vertr.Domain;
using Vertr.Domain.Enums;

namespace Vertr.Domain;
public record class OrderState
{
    public string OrderId { get; set; } = string.Empty;
    public OrderExecutionReportStatus ExecutionReportStatus { get; set; }

    public long LotsRequested { get; set; }

    public long LotsExecuted { get; set; }

    public decimal InitialOrderPrice { get; set; }

    public decimal ExecutedOrderPrice { get; set; }

    public decimal TotalOrderAmount { get; set; }

    public decimal AveragePositionPrice { get; set; }

    public decimal InitialCommission { get; set; }

    public decimal ExecutedCommission { get; set; }

    public OrderDirection Direction { get; set; }

    public decimal InitialSecurityPrice { get; set; }

    public decimal ServiceCommission { get; set; }

    public string Currency { get; set; }

    public OrderType OrderType { get; set; }

    public DateTime OrderDate { get; set; }

    public string InstrumentUid { get; set; }

    public string OrderRequestId { get; set; }
}
