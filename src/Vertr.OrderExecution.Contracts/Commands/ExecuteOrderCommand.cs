using MediatR;

namespace Vertr.OrderExecution.Contracts.Commands;

public class ExecuteOrderCommand : OrderCommandBase, IRequest<ExecuteOrderResponse>
{
    public long QtyLots { get; init; }
}
