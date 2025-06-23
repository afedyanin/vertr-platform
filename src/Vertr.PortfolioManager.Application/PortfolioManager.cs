using Vertr.PortfolioManager.Application.Abstractions;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;
using Vertr.TinvestGateway.Contracts.Interfaces;

namespace Vertr.PortfolioManager.Application;

internal class PortfolioManager : IPortfolioManager
{
    // TODO: Move it into DB
    private static readonly string[] _accounts =
        [
            "b883ab13-997b-4823-9698-20bac64aeaad"
        ];

    private readonly IPortfolioSnapshotRepository _portfolioSnapshotRepository;
    private readonly IOperationEventRepository _operationEventRepository;
    private readonly ITinvestGatewayAccounts _tinvestGateway;

    public PortfolioManager(
        IPortfolioSnapshotRepository portfolioSnapshotRepository,
        IOperationEventRepository operationEventRepository,
        ITinvestGatewayAccounts tinvestGateway)
    {
        _portfolioSnapshotRepository = portfolioSnapshotRepository;
        _operationEventRepository = operationEventRepository;
        _tinvestGateway = tinvestGateway;
    }

    public Task<string[]> GetActiveAccounts()
        => Task.FromResult(_accounts);
 
    public Task<PortfolioSnapshot?> GetLastPortfolio(string accountId, Guid? bookId = null)
        => _portfolioSnapshotRepository.GetLast(accountId, bookId);

    public Task<PortfolioSnapshot[]> GetPortfolioHistory(string accountId, Guid? bookId = null, int maxRecords = 100)
        => _portfolioSnapshotRepository.GetHistory(accountId, bookId, maxRecords);

    public async Task<PortfolioSnapshot?> MakeSnapshot(string accountId, Guid? bookId = null)
    {
        var snapshot = await _tinvestGateway.GetPortfolio(accountId, bookId);

        if (snapshot != null)
        {
            var saved = await _portfolioSnapshotRepository.Save(snapshot);
        }

        return snapshot;
    }

    public async Task Delete(string accountId, Guid? bookId = null)
    {
        if (bookId != null)
        {
            _ = await _operationEventRepository.DeleteByBookId(bookId.Value);
            _ = await _portfolioSnapshotRepository.DeleteByBookId(bookId.Value);

            return;
        }

        _ = await _operationEventRepository.DeleteByAccountId(accountId);
        _ = await _portfolioSnapshotRepository.DeleteByAccountId(accountId);
    }
}
