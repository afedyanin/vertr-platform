using MediatR;
using Vertr.Domain.Enums;

namespace Vertr.Application.Orders;
public class ExecuteOrderRequest : IRequest
{
    public required string AccountId { get; init; }
    public required Guid InstrumentId { get; init; }
    public required Guid RequestId { get; init; }
    public Guid? TradingSignalId { get; init; }
    public required OrderDirection OrderDirection { get; init; }
    public required OrderType OrderType { get; init; }
    public required TimeInForceType TimeInForceType { get; init; }
    public required PriceType PriceType { get; init; }
    public required decimal Price { get; init; }
    public required long QuantityLots { get; init; }
}
