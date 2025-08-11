using Vertr.Platform.Common.Mediator;
using Vertr.OrderExecution.Contracts.Commands;

namespace Vertr.OrderExecution.Contracts.Requests;

public class ClosePositionCommand : OrderCommandBase, IRequest<ExecuteOrderResponse>
{
}
