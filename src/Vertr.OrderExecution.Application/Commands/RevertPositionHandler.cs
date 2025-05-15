using MediatR;

namespace Vertr.OrderExecution.Application.Commands;
internal class RevertPositionHandler : IRequestHandler<RevertPositionRequest, RevertPositionResponse>
{
    public Task<RevertPositionResponse> Handle(RevertPositionRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
