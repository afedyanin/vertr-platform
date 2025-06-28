using Microsoft.Extensions.Options;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.PortfolioManager.Application.Repositories;

internal class PortfolioRepository : IPortfolioRepository
{
    private readonly Dictionary<PortfolioIdentity, Portfolio> _portfolios = [];

    private readonly PortfolioSettings _portfolioSettings;

    public PortfolioRepository(IOptions<PortfolioSettings> options)
    {
        _portfolioSettings = options.Value;
    }

    public string[] GetActiveAccounts() => [.. _portfolioSettings.Accounts];

    public Portfolio[] GetAllPortfolios() => [.. _portfolios.Values];

    public Portfolio? GetPortfolio(PortfolioIdentity portfolioIdentity)
    {
        _portfolios.TryGetValue(portfolioIdentity, out var portfolio);
        return portfolio;
    }
    public void Save(Portfolio portfolio)
    {
        _portfolios[portfolio.Identity] = portfolio;
    }

    public void Delete(PortfolioIdentity portfolioIdentity)
    {
        _portfolios.Remove(portfolioIdentity);
    }
}
