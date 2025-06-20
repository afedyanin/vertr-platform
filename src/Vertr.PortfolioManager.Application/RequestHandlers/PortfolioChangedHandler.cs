using MediatR;
using Vertr.PortfolioManager.Application.Abstractions;
using Vertr.PortfolioManager.Contracts.Requests;

namespace Vertr.PortfolioManager.Application.RequestHandlers;

internal class PortfolioChangedHandler : IRequestHandler<PortfolioChangedRequest>
{
    private readonly IPortfolioSnapshotRepository _portfolioSnapshotRepository;

    public PortfolioChangedHandler(IPortfolioSnapshotRepository portfolioSnapshotRepository)
    {
        _portfolioSnapshotRepository = portfolioSnapshotRepository;
    }

    public async Task Handle(PortfolioChangedRequest request, CancellationToken cancellationToken)
    {
        var snapshot = request.Portfolio;

        if (snapshot == null)
        {
            return;
        }

        var saved = await _portfolioSnapshotRepository.Save(snapshot);
    }
}
