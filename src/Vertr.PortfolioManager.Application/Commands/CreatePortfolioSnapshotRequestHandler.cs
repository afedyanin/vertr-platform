using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.PortfolioManager.Application.Abstractions;
using Vertr.PortfolioManager.Application.Converters;
using Vertr.OrderExecution.Contracts.Interfaces;

namespace Vertr.PortfolioManager.Application.Commands;

internal class CreatePortfolioSnapshotRequestHandler : IRequestHandler<CreatePortfolioSnapshotRequest, CreatePortfolioSnapshotResponse>
{
    private readonly IPortfolioSnapshotRepository _snapshotRepository;
    private readonly ITinvestGateway _tinvestGateway;
    private readonly ILogger<CreatePortfolioSnapshotRequestHandler> _logger;

    public CreatePortfolioSnapshotRequestHandler(
        IPortfolioSnapshotRepository snapshotRepository,
        ITinvestGateway tinvestGateway,
        ILogger<CreatePortfolioSnapshotRequestHandler> logger
        )
    {
        _snapshotRepository = snapshotRepository;
        _tinvestGateway = tinvestGateway;
        _logger = logger;
    }

    public async Task<CreatePortfolioSnapshotResponse> Handle(CreatePortfolioSnapshotRequest request, CancellationToken cancellationToken)
    {
        var portfolioResponse = await _tinvestGateway.GetPortfolio(request.AccountId);
        var snapshot = portfolioResponse.Convert(request.BookId);

        if (snapshot != null)
        {
            var saved = await _snapshotRepository.Save(snapshot);
        }

        var response = new CreatePortfolioSnapshotResponse
        {
            Snapshot = snapshot,
        };

        return response;
    }
}
