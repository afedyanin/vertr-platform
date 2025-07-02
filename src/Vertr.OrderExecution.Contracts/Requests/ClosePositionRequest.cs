using MediatR;

namespace Vertr.OrderExecution.Contracts.Requests;

public class ClosePositionRequest : OrderRequestBase, IRequest<ExecuteOrderResponse>
{
}
