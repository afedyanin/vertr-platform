using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Abstractions;

public interface IRatesRepository
{
    InterestRate[] GetAll(string ticker);
    InterestRate GetLast(string ticker, DateTime? time = null);
    Task Load(string[] tickers);
    string ToJson();
    Task FromJson(string json);
}
