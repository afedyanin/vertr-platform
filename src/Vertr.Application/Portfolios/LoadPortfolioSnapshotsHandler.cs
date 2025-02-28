using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.Domain.Ports;
using Vertr.Domain.Repositories;

namespace Vertr.Application.Portfolios;

internal class LoadPortfolioSnapshotsHandler : IRequestHandler<LoadPortfolioSnapshotsRequest>
{
    private readonly ITinvestPortfolioRepository _repository;
    private readonly ITinvestGateway _gateway;

    private readonly ILogger<LoadPortfolioSnapshotsHandler> _logger;

    public LoadPortfolioSnapshotsHandler(
        ITinvestPortfolioRepository repository,
        ITinvestGateway gateway,
        ILogger<LoadPortfolioSnapshotsHandler> logger)
    {
        _repository = repository;
        _gateway = gateway;
        _logger = logger;
    }

    public async Task Handle(LoadPortfolioSnapshotsRequest request, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();

        foreach (var accountId in request.Accounts)
        {
            tasks.Add(LoadPortfolios(accountId, cancellationToken));
        }

        await Task.WhenAll(tasks);

        _logger.LogDebug($"Loading T-Invest portfolios for {request.Accounts.Count()} accounts completed.");
    }

    internal async Task LoadPortfolios(string accountId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrEmpty(accountId))
        {
            throw new ArgumentNullException(nameof(accountId));
        }

        try
        {
            _logger.LogDebug($"Loading portfolio for {accountId}");
            var portfolio = await _gateway.GetPortfolio(accountId);
            var insetred = await _repository.Insert(portfolio, cancellationToken);
            _logger.LogDebug($"{insetred} portfolio snapshots inserted for {accountId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "T-Invest portfolio loading error.");
        }
    }
}
