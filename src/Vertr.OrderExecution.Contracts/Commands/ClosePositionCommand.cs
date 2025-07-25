using Vertr.OrderExecution.Contracts.Commands;
using Vertr.Platform.Common.Mediator;

namespace Vertr.OrderExecution.Contracts.Requests;

public class ClosePositionCommand : OrderCommandBase, ICommand<ExecuteOrderResponse>
{
}
