using Vertr.Platform.Common.Mediator;

namespace Vertr.OrderExecution.Contracts.Commands;

public class ClosePositionRequest : OrderCommandBase, IRequest<ExecuteOrderResponse>
{
}
