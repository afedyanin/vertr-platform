using Vertr.Platform.Common.Mediator;

namespace Vertr.OrderExecution.Contracts.Commands;

public class ReversePositionCommand : OrderCommandBase, IRequest<ExecuteOrderResponse>
{
}
