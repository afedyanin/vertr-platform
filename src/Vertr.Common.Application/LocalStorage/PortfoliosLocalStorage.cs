using System.Collections.ObjectModel;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.LocalStorage;

internal sealed class PortfoliosLocalStorage : IPortfoliosLocalStorage
{
    private readonly Dictionary<string, Guid> _names = [];
    private readonly Dictionary<Guid, Portfolio> _portfolios = [];

    public Portfolio? GetById(Guid portfolioId)
    {
        _portfolios.TryGetValue(portfolioId, out var portfolio);
        return portfolio;
    }

    public Portfolio? GetByName(string name)
    {
        if (!_names.TryGetValue(name, out var portfolioId))
        {
            return null;
        }

        _portfolios.TryGetValue(portfolioId, out var portfolio);

        return portfolio;
    }

    public ReadOnlyDictionary<string, Portfolio> GetAll()
    {
        var res = new Dictionary<string, Portfolio>();

        foreach (var kvp in _names)
        {
            _portfolios.TryGetValue(kvp.Value, out var pfolio);

            if (pfolio != null)
            {
                res[kvp.Key] = pfolio;
            }
        }

        return res.AsReadOnly();
    }

    public string GetNameById(Guid portfolioId)
    {
        foreach (var kvp in _names)
        {
            if (kvp.Value == portfolioId)
            {
                return kvp.Key;
            }
        }

        return string.Empty;
    }


    public void Init(string[] names)
    {
        _portfolios.Clear();
        _names.Clear();

        foreach (var name in names.Distinct())
        {
            var portfolio = new Portfolio
            {
                Id = Guid.NewGuid(),
                UpdatedAt = DateTime.UtcNow,
            };

            _portfolios[portfolio.Id] = portfolio;
            _names[name] = portfolio.Id;
        }
    }

    public bool Update(Portfolio portfolio)
    {
        if (!_portfolios.ContainsKey(portfolio.Id))
        {
            return false;
        }

        _portfolios[portfolio.Id] = portfolio;
        return true;
    }
}

