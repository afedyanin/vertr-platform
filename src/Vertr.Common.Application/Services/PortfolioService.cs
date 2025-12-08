using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Services;

// TODO Implement portfolio storage by predictor
internal sealed class PortfolioService : IPortfolioService
{
    private readonly Dictionary<Guid, Portfolio> _portfolios = [];

    public Portfolio[] GetAll()
    {
        return [.. _portfolios.Values];
    }

    public Portfolio? GetById(Guid portfolioId)
    {
        _portfolios.TryGetValue(portfolioId, out var portfolio);
        return portfolio;
    }

    public void Update(Portfolio portfolio)
    {
        _portfolios[portfolio.Id] = portfolio;
    }
}

public interface IPortfolioService
{
    public Portfolio[] GetAll();

    public Portfolio? GetById(Guid portfolioId);

    public void Update(Portfolio portfolio);
}
