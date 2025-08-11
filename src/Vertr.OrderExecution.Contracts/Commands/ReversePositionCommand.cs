using MediatR;

namespace Vertr.OrderExecution.Contracts.Commands;

public class ReversePositionCommand : OrderCommandBase, IRequest<ExecuteOrderResponse>
{
}
