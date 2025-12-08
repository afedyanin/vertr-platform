using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Services;

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

    public Portfolio? GetByPredictor(string predictor)
        => _portfolios.Values
            .FirstOrDefault(p => p.Predictor.Equals(predictor, StringComparison.OrdinalIgnoreCase));

    public string[] GetPredictors()
        => [.. _portfolios.Values.Select(p => p.Predictor)];

    public void Init(string[] precitors)
    {
        foreach (var precitor in precitors.Distinct())
        {
            var portfolio = new Portfolio
            {
                Id = Guid.NewGuid(),
                UpdatedAt = DateTime.UtcNow,
                Predictor = precitor,
            };

            _portfolios[portfolio.Id] = portfolio;
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

public interface IPortfolioService
{
    public Portfolio[] GetAll();

    public Portfolio? GetById(Guid portfolioId);

    public Portfolio? GetByPredictor(string predictor);

    public void Update(Portfolio portfolio);

    public void Init(string[] precitors);

    public string[] GetPredictors();
}
