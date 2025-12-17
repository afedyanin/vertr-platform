using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.LocalStorage;

internal sealed class CandlesLocalStorage : ICandlesLocalStorage
{
    private readonly Dictionary<Guid, SortedList<DateTime, Candle>> _candles = new Dictionary<Guid, SortedList<DateTime, Candle>>();

    public int CandlesBufferLength { get; set; } = 100;

    public void Load(IEnumerable<Candle> candles)
    {
        foreach (var candle in candles.OrderBy(c => c.TimeUtc))
        {
            Update(candle, recalculateStats: false);
        }
    }

    public void Update(Candle candle, bool recalculateStats = true)
    {
        if (!_candles.TryGetValue(candle.InstrumentId, out var list))
        {
            list = [];
            _candles[candle.InstrumentId] = list;
        }

        list[candle.TimeUtc] = candle;

        if (list.Count > CandlesBufferLength)
        {
            list.Remove(list.First().Key);
        }
    }

    public int GetCount(Guid instrumentId)
        => _candles.TryGetValue(instrumentId, out var candleList) ? candleList.Count : 0;

    public Candle[] Get(Guid instrumentId)
        => _candles.TryGetValue(instrumentId, out var candleList) ? [.. candleList.Values] : [];
}
