using Vertr.Platform.Common;

namespace Vertr.OrderExecution.Contracts.Commands;

public class OpenPositionCommand : OrderCommandBase, ICommand<ExecuteOrderResponse>
{
    public long QtyLots { get; init; }
}
