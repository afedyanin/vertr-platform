using MediatR;

namespace Vertr.OrderExecution.Contracts.Requests;

public class ReversePositionRequest : OrderRequestBase, IRequest<ExecuteOrderResponse>
{
}
