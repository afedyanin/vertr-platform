using Vertr.OrderExecution.Contracts.Commands;
using Vertr.Platform.Common;

namespace Vertr.OrderExecution.Contracts.Requests;

public class ClosePositionCommand : OrderCommandBase, ICommand<ExecuteOrderResponse>
{
}
