using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Abstractions;

public interface IIndexRatesRepository
{
    public IndexRate[] GetAll(string ticker);
    public IndexRate? GetLast(string ticker, DateTime? time = null);
    public void Update(IndexRate rate);
    public void Load(IndexRate[] rates);
}
