using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Abstractions;

public interface IFutureInfoRepository
{
    public FutureInfo[] GetAll(string? tickerWildCard = null);

    public FutureInfo? Get(string ticker);

    public void Load(FutureInfo[] futures);
}
