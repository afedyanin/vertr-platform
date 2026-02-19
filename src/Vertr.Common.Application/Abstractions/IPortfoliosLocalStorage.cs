using System.Collections.ObjectModel;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Abstractions;

public interface IPortfoliosLocalStorage
{
    public ReadOnlyDictionary<string, Portfolio> GetAll();

    public Portfolio? GetById(Guid portfolioId);

    public string GetNameById(Guid portfolioId);

    public Portfolio? GetByName(string name);

    public bool Update(Portfolio portfolio);

    public void Init(string[] precitors);
}
