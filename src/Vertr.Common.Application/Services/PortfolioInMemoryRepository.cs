using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Services;

internal sealed class PortfolioInMemoryRepository : IPortfolioRepository
{
    private readonly Dictionary<string, Guid> _predictors = [];
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

    public Portfolio? GetByPredictor(string predictor)
    {
        if (!_predictors.TryGetValue(predictor, out var portfolioId))
        {
            return null;
        }

        _portfolios.TryGetValue(portfolioId, out var portfolio);

        return portfolio;
    }

    public string[] GetPredictors()
        => [.. _predictors.Keys];

    public void Init(string[] precitors)
    {
        foreach (var precitor in precitors.Distinct())
        {
            var portfolio = new Portfolio
            {
                Id = Guid.NewGuid(),
                UpdatedAt = DateTime.UtcNow,
            };

            _portfolios[portfolio.Id] = portfolio;
            _predictors[precitor] = portfolio.Id;
        }
    }

    public void Update(Portfolio portfolio)
    {
        if (_portfolios.ContainsKey(portfolio.Id))
        {
            _portfolios[portfolio.Id] = portfolio;
        }
    }
}

public interface IPortfolioRepository
{
    public Portfolio[] GetAll();

    public Portfolio? GetById(Guid portfolioId);

    public Portfolio? GetByPredictor(string predictor);

    public void Update(Portfolio portfolio);

    public void Init(string[] precitors);

    public string[] GetPredictors();
}
