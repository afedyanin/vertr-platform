using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Abstractions;

public interface IFutureInfoRepository
{
    FutureInfo[] GetAll(string? tickerWildCard = null);

    FutureInfo Get(string ticker);

    Task Load(string[] tickers);

    string ToJson();

    Task FromJson(string json);
}
