using MediatR;

namespace Vertr.OrderExecution.Contracts.Requests;

public class ExecuteOrderRequest : OrderRequestBase, IRequest<ExecuteOrderResponse>
{
    public long QtyLots { get; init; }
}
