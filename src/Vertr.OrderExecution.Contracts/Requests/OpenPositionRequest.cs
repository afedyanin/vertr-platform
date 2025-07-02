using MediatR;

namespace Vertr.OrderExecution.Contracts.Requests;

public class OpenPositionRequest : OrderRequestBase, IRequest<ExecuteOrderResponse>
{
    public long QtyLots { get; init; }
}
