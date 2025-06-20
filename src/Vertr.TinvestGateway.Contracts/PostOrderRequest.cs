using Vertr.OrderExecution.Contracts.Enums;

namespace Vertr.TinvestGateway.Contracts;
public record class PostOrderRequest
{
    public required string AccountId { get; init; }
    public required Guid InstrumentId { get; init; }
    public required Guid RequestId { get; init; }
    public required OrderDirection OrderDirection { get; init; }
    public required OrderType OrderType { get; init; }
    public required TimeInForceType TimeInForceType { get; init; }
    public required PriceType PriceType { get; init; }
    public required decimal Price { get; init; }
    public required long QuantityLots { get; init; }
}
