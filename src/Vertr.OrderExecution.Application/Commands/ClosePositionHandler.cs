using MediatR;

namespace Vertr.OrderExecution.Application.Commands;
internal class ClosePositionHandler : IRequestHandler<ClosePositionRequest, ClosePositionResponse>
{
    public Task<ClosePositionResponse> Handle(ClosePositionRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
