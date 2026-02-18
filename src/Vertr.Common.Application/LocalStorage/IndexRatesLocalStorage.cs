using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.LocalStorage;

internal class IndexRatesLocalStorage : IIndexRatesRepository
{
    private readonly Dictionary<string, SortedList<DateTime, IndexRate>> _rates = [];

    public IndexRate[] GetAll(string ticker)
    {
        _rates.TryGetValue(ticker, out var list);
        return list?.Values.ToArray() ?? [];
    }

    public IndexRate? GetLast(string ticker, DateTime? time = null)
    {
        if (!_rates.TryGetValue(ticker, out var list))
        {
            return null;
        }

        if (time == null)
        {
            return list.Last().Value;
        }

        return list.Last(t => t.Key <= time).Value;
    }

    public void Update(IndexRate rate)
    {
        _rates.TryGetValue(rate.Ticker, out var list);

        if (list == null)
        {
            list = [];
            _rates[rate.Ticker] = list;
        }

        list[rate.Time] = rate;
    }

    public void Load(IndexRate[] rates)
    {
        foreach (var rate in rates)
        {
            Update(rate);
        }
    }
}
