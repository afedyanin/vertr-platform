using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.PortfolioManager.Application;

internal class PortfolioProvider : IPortfolioProvider
{
    private readonly Dictionary<PortfolioIdentity, Portfolio> _portfolios = [];

    public Portfolio[] GetAllPortfolios() => [.. _portfolios.Values];

    public Portfolio? GetPortfolio(PortfolioIdentity portfolioIdentity)
    {
        _portfolios.TryGetValue(portfolioIdentity, out var portfolio);
        return portfolio;
    }
    public void Update(Portfolio portfolio)
    {
        _portfolios[portfolio.Identity] = portfolio;
    }

    public void Remove(PortfolioIdentity portfolioIdentity)
    {
        _portfolios.Remove(portfolioIdentity);
    }

    public Position? GetPosition(PortfolioIdentity portfolioIdentity, Guid instrumentId)
    {
        var portfolio = GetPortfolio(portfolioIdentity);

        if (portfolio == null)
        {
            return null;
        }

        var position = portfolio.Positions.SingleOrDefault(p => p.InstrumentId == instrumentId);

        return position;
    }
}
