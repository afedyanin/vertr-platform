using Vertr.Platform.Common.Mediator;

namespace Vertr.OrderExecution.Contracts.Commands;

public class ReversePositionRequest : OrderCommandBase, IRequest<ExecuteOrderResponse>
{
}
