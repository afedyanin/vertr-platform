using System.Text.Json;
using Vertr.TinvestGateway.Contracts.Orders.Enums;
using Vertr.TinvestGateway.Contracts.Portfolios;

namespace Vertr.TinvestGateway.Contracts.Orders;

public record class PostOrderResponse
{
    public string OrderId { get; init; } = string.Empty;

    public string OrderRequestId { get; init; } = string.Empty;

    public OrderExecutionReportStatus ExecutionReportStatus { get; init; }

    public OrderType OrderType { get; init; }

    public OrderDirection OrderDirection { get; init; }

    public long LotsRequested { get; init; }

    public long LotsExecuted { get; init; }

    public Money? InitialOrderPrice { get; init; }

    public Money? ExecutedOrderPrice { get; init; }

    public Money? TotalOrderAmount { get; init; }

    public Money? InitialCommission { get; init; }

    public Money? ExecutedCommission { get; init; }

    public Money? InitialSecurityPrice { get; init; }

    public string Message { get; init; } = string.Empty;

    public Guid InstrumentId { get; init; }

    public string ToJson() => JsonSerializer.Serialize(this, JsonOptions.DefaultOptions);

    public static PostOrderResponse? FromJson(string json) => JsonSerializer.Deserialize<PostOrderResponse>(json, JsonOptions.DefaultOptions);
}