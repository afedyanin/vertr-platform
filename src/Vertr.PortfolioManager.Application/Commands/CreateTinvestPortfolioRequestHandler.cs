using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.PortfolioManager.Application.Abstractions;
using Vertr.PortfolioManager.Application.Converters;
using Vertr.TinvestGateway.Contracts;

namespace Vertr.PortfolioManager.Application.Commands;

internal class CreateTinvestPortfolioRequestHandler : IRequestHandler<CreateTinvestPortfolioRequest, CreateTinvestPortfolioResponse>
{
    private readonly IPortfolioSnapshotRepository _snapshotRepository;
    private readonly IPortfolioMetadataRepository _metadataRepository;
    private readonly ITinvestGateway _tinvestGateway;
    private readonly ILogger<CreateTinvestPortfolioRequestHandler> _logger;

    public CreateTinvestPortfolioRequestHandler(
        IPortfolioSnapshotRepository snapshotRepository,
        IPortfolioMetadataRepository metadataRepository,
        ITinvestGateway tinvestGateway,
        ILogger<CreateTinvestPortfolioRequestHandler> logger
        )
    {
        _metadataRepository = metadataRepository;
        _snapshotRepository = snapshotRepository;
        _tinvestGateway = tinvestGateway;
        _logger = logger;
    }

    public async Task<CreateTinvestPortfolioResponse> Handle(CreateTinvestPortfolioRequest request, CancellationToken cancellationToken)
    {
        var portfolioId = request.PortfolioId;

        var metadata = await _metadataRepository.GetById(portfolioId);

        if (metadata == null)
        {
            // TODO: Use error codes
            throw new ArgumentException($"Metadata not found by portfolioId={portfolioId}");
        }

        if (metadata.PortfolioType != Entities.PortfolioType.Tinvest)
        {
            // TODO: Use error codes
            throw new ArgumentException($"Unsupported portfolio type for portfolioId={portfolioId}");
        }

        var portfolioResponse = await _tinvestGateway.GetPortfolio(metadata.AccountId);
        var snapshot = portfolioResponse.Convert(portfolioId);

        if (snapshot != null)
        {
            var saved = await _snapshotRepository.Save(snapshot);
        }

        var response = new CreateTinvestPortfolioResponse
        {
            Snapshot = snapshot,
        };

        return response;
    }
}
