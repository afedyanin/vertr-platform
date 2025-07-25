using Vertr.Platform.Common.Mediator;

namespace Vertr.OrderExecution.Contracts.Commands;

public class ExecuteOrderCommand : OrderCommandBase, ICommand<ExecuteOrderResponse>
{
    public long QtyLots { get; init; }
}
