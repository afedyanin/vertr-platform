using MediatR;
using Vertr.Domain;

namespace Vertr.Application.Orders;
public class OrderApprovementRequest : IRequest<OrderApprovementResponse>
{
    public required TradingSignal Signal { get; init; }

    public required string AccountId { get; init; }
}
