using MediatR;

namespace Vertr.OrderExecution.Contracts.Requests;

public class OrderTradesRequest : IRequest
{
    public OrderTrades? OrderTrades { get; init; }
}
