using Vertr.Platform.Common.Mediator;

namespace Vertr.OrderExecution.Contracts.Commands;

public class ClosePositionCommand : OrderCommandBase, IRequest<ExecuteOrderResponse>
{
}
