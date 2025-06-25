using Vertr.OrderExecution.Contracts;
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
 
    public Task<PortfolioSnapshot?> GetPortfolio(PortfolioIdentity portfolioIdentity)
        => _portfolioRepository.GetPortfolio(portfolioIdentity);

    public async Task Remove(PortfolioIdentity portfolioIdentity)
    {
        await _portfolioRepository.Remove(portfolioIdentity);
        await _operationEventRepository.Delete(portfolioIdentity);
    }
}
