using MediatR;
using Vertr.PortfolioManager.Contracts.Requests;

namespace Vertr.PortfolioManager.Application.RequestHandlers;
internal class PositionOverridesHandler : IRequestHandler<PositionOverridesRequest>
{
    public Task Handle(PositionOverridesRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
