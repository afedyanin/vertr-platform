using System.Text.Json;
using Vertr.TinvestGateway.Contracts.Orders.Enums;

namespace Vertr.TinvestGateway.Contracts.Orders;

public record class PostOrderRequest
{
    public required Guid InstrumentId { get; init; }
    public required Guid RequestId { get; init; }
    public required Guid PortfolioId { get; init; }
    public required OrderDirection OrderDirection { get; init; }
    public required OrderType OrderType { get; init; }
    public required TimeInForceType TimeInForceType { get; init; }
    public required PriceType PriceType { get; init; }
    public required decimal Price { get; init; }
    public required long QuantityLots { get; init; }
    public DateTime CreatedAt { get; init; }

    public string ToJson() => JsonSerializer.Serialize(this, JsonOptions.DefaultOptions);
    public static PostOrderRequest? FromJson(string json) => JsonSerializer.Deserialize<PostOrderRequest>(json, JsonOptions.DefaultOptions);
}