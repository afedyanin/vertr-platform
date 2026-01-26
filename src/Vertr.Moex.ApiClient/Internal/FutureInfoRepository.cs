using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;

namespace Vertr.Moex.ApiClient.Internal;

internal class FutureInfoRepository : IFutureInfoRepository
{
    public Task FromJson(string json)
    {
        throw new NotImplementedException();
    }

    public FutureInfo Get(string ticker)
    {
        throw new NotImplementedException();
    }

    public FutureInfo[] GetAll(string? tickerWildCard = null)
    {
        throw new NotImplementedException();
    }

    public Task Load(string[] tickers)
    {
        throw new NotImplementedException();
    }

    public string ToJson()
    {
        throw new NotImplementedException();
    }
}
