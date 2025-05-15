using MediatR;

namespace Vertr.OrderExecution.Application.Commands;
public class ClosePositionRequest : IRequest<ClosePositionResponse>
{
}
