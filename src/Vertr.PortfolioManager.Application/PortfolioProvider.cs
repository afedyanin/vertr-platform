using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.PortfolioManager.Application;

internal class PortfolioProvider : IPortfolioProvider
{
    private readonly Dictionary<Guid, Portfolio> _portfolios = [];

    public Portfolio[] GetAllPortfolios() => [.. _portfolios.Values];

    public Portfolio? GetPortfolio(Guid portfolioId)
    {
        _portfolios.TryGetValue(portfolioId, out var portfolio);
        return portfolio;
    }
    public void Update(Portfolio portfolio)
    {
        _portfolios[portfolio.Id] = portfolio;
    }

    public void Remove(Guid portfolioId)
    {
        _portfolios.Remove(portfolioId);
    }

    public Position? GetPosition(Guid portfolioId, Guid instrumentId)
    {
        var portfolio = GetPortfolio(portfolioId);

        if (portfolio == null)
        {
            return null;
        }

        var position = portfolio.Positions.SingleOrDefault(p => p.InstrumentId == instrumentId);

        return position;
    }
}
