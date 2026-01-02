using System.Collections.ObjectModel;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.LocalStorage;

internal sealed class PortfoliosLocalStorage : IPortfoliosLocalStorage
{
    private readonly Dictionary<string, Guid> _predictors = [];
    private readonly Dictionary<Guid, Portfolio> _portfolios = [];

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

    public ReadOnlyDictionary<string, Portfolio> GetAll()
    {
        var res = new Dictionary<string, Portfolio>();

        foreach (var kvp in _predictors)
        {
            _portfolios.TryGetValue(kvp.Value, out var pfolio);

            if (pfolio != null)
            {
                res[kvp.Key] = pfolio;
            }
        }

        return res.AsReadOnly();
    }

    public string GetPredictor(Guid portfolioId)
    {
        foreach (var kvp in _predictors)
        {
            if (kvp.Value == portfolioId)
            {
                return kvp.Key;
            }
        }

        return string.Empty;
    }


    public void Init(string[] precitors)
    {
        _portfolios.Clear();
        _predictors.Clear();

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

