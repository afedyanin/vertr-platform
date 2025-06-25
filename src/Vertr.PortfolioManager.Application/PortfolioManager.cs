using Vertr.PortfolioManager.Application.Abstractions;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.PortfolioManager.Application;

internal class PortfolioManager : IPortfolioManager
{
    // TODO: Move it into DB
    private static readonly string[] _accounts =
        [
            "b883ab13-997b-4823-9698-20bac64aeaad"
        ];

    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IOperationEventRepository _operationEventRepository;

    public PortfolioManager(
        IPortfolioRepository portfolioRepository,
        IOperationEventRepository operationEventRepository)
    {
        _portfolioRepository = portfolioRepository;
        _operationEventRepository = operationEventRepository;
    }

    public Task<string[]> GetActiveAccounts()
        => Task.FromResult(_accounts);
 
    public Task<PortfolioSnapshot?> GetPortfolio(string accountId, Guid? bookId = null)
        => _portfolioRepository.GetPortfolio(accountId, bookId);

    public async Task DeleteOperationEvents(string accountId, Guid? bookId = null)
    {
        if (bookId == null)
        {
            _ = await _operationEventRepository.DeleteByAccountId(accountId);
            return;
        }

        _ = await _operationEventRepository.DeleteByBookId(bookId.Value);
    }
}
