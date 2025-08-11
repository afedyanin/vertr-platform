using Vertr.Platform.Common.Mediator;

namespace Vertr.OrderExecution.Contracts.Commands;

public class OpenPositionRequest : OrderCommandBase, IRequest<ExecuteOrderResponse>
{
    public long QtyLots { get; init; }
}
