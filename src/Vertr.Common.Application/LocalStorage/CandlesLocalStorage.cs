using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.LocalStorage;

internal sealed class CandlesLocalStorage : ICandlesLocalStorage
{
    private readonly Dictionary<Guid, SortedList<DateTime, Candle>> _candles = new Dictionary<Guid, SortedList<DateTime, Candle>>();

    private readonly Dictionary<Guid, PriceStats> _candleStats = [];

    public int MaxCandlesCount { get; set; } = 100;

    public void Load(IEnumerable<Candle> candles)
    {
        var instruments = new HashSet<Guid>();

        foreach (var candle in candles.OrderBy(c => c.TimeUtc))
        {
            Update(candle, recalculateStats: false);
            instruments.Add(candle.InstrumentId);
        }

        foreach (var instrumentId in instruments)
        {
            RecalculateStats(instrumentId);
        }
    }

    public void Update(Candle candle, bool recalculateStats = true)
    {
        _candles.TryGetValue(candle.InstrumentId, out var list);
        list ??= [];
        list[candle.TimeUtc] = candle;

        if (list.Count > MaxCandlesCount)
        {
            list.Remove(list.First().Key);
        }

        if (recalculateStats)
        {
            RecalculateStats(candle.InstrumentId);
        }
    }

    public int GetCount(Guid instrumentId)
        => _candles.TryGetValue(instrumentId, out var candleList) ? candleList.Count : 0;

    public Candle[] Get(Guid instrumentId)
        => _candles.TryGetValue(instrumentId, out var candleList) ? [.. candleList.Values] : [];

    public PriceStats? GetStats(Guid instrumentId)
        => _candleStats.TryGetValue(instrumentId, out var stats) ? stats : null;

    private void RecalculateStats(Guid instrumentId)
    {
        _candles.TryGetValue(instrumentId, out var candles);

        if (candles == null)
        {
            return;
        }

        var count = candles.Count;

        if (count <= 0)
        {
            return;
        }

        var close = candles.Values.Select(c => c.Close);
        var avg = close.Average();
        var sum = close.Sum(d => (d - avg) * (d - avg));
        var dev = Math.Sqrt((double)sum / count);

        _candleStats[instrumentId] = new PriceStats
        {
            Mean = (double)avg,
            StdDev = dev,
        };
    }
}
