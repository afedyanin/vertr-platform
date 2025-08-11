using MediatR;

namespace Vertr.OrderExecution.Contracts.Commands;

public class OpenPositionCommand : OrderCommandBase, IRequest<ExecuteOrderResponse>
{
    public long QtyLots { get; init; }
}
