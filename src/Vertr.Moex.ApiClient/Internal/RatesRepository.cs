using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;

namespace Vertr.Moex.ApiClient.Internal;

internal class RatesRepository : IRatesRepository
{
    public Task FromJson(string json)
    {
        throw new NotImplementedException();
    }

    public InterestRate[] GetAll(string ticker)
    {
        throw new NotImplementedException();
    }

    public InterestRate GetLast(string ticker, DateTime? time = null)
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
