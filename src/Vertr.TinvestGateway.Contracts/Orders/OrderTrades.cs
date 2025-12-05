using System.Text.Json;
using Vertr.TinvestGateway.Contracts.Orders.Enums;

namespace Vertr.TinvestGateway.Contracts.Orders;

public record class OrderTrades
{
    public required Guid Id { get; init; }

    public string OrderId { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public OrderDirection Direction { get; init; }

    public Guid InstrumentId { get; init; }

    public Trade[] Trades { get; init; } = [];

    public string ToJson() => JsonSerializer.Serialize(this, JsonOptions.DefaultOptions);

    public static OrderTrades? FromJson(string json) => JsonSerializer.Deserialize<OrderTrades>(json, JsonOptions.DefaultOptions);
}