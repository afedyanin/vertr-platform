using MediatR;

namespace Vertr.OrderExecution.Application.Commands;
internal class OpenPositionHandler : IRequestHandler<OpenPositionRequest, OpenPositionResponse>
{
    public Task<OpenPositionResponse> Handle(OpenPositionRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
