using System.Collections.ObjectModel;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Abstractions;

public interface IPortfoliosLocalStorage
{
    public Portfolio[] GetAll();

    public Portfolio? GetById(Guid portfolioId);

    public Portfolio? GetByPredictor(string predictor);

    public void Update(Portfolio portfolio);

    public void Init(string[] precitors);

    public ReadOnlyDictionary<string, Guid> GetPredictors();
}
