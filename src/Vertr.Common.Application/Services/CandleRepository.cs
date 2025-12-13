using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Services;

internal sealed class CandleRepository : ICandleRepository
{
    private readonly Dictionary<Guid, LinkedList<Candle>> _candles = new Dictionary<Guid, LinkedList<Candle>>();

    private readonly Dictionary<Guid, PriceStats> _candleStats = [];

    public int MaxCandlesCount { get; set; } = 100;

    public void Load(IEnumerable<Candle> candles)
    {
        var instruments = new HashSet<Guid>();

        foreach (var candle in candles.OrderBy(c => c.TimeUtc))
        {
            instruments.Add(candle.InstrumentId);
            Update(candle, false);
        }

        foreach (var instrumentId in instruments)
        {
            RecalculateStats(instrumentId);
        }
    }

    public bool Update(Candle candle, bool recalculateStats = true)
    {
        _candles.TryGetValue(candle.InstrumentId, out var list);

        list ??= new LinkedList<Candle>();

        var last = list.Last;

        if (last == null || last.Value.TimeUtc > candle.TimeUtc)
        {
            return false;
        }

        list.AddLast(candle);

        if (list.Count > MaxCandlesCount)
        {
            list.RemoveFirst();
        }

        if (recalculateStats)
        {
            RecalculateStats(candle.InstrumentId);
        }

        return true;
    }

    public int GetCount(Guid instrumentId)
        => _candles.TryGetValue(instrumentId, out var candleList) ? candleList.Count : 0;

    public Candle[] Get(Guid instrumentId)
        => _candles.TryGetValue(instrumentId, out var candleList) ? [.. candleList] : [];

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

        var avg = candles.Average(c => c.Close);
        var sum = candles.Sum(d => (d.Close - avg) * (d.Close - avg));
        var dev = Math.Sqrt((double)sum / count);

        _candleStats[instrumentId] = new PriceStats
        {
            Mean = (double)avg,
            StdDev = dev,
        };
    }
}

public interface ICandleRepository
{
    public int MaxCandlesCount { get; set; }

    public void Load(IEnumerable<Candle> candles);

    public bool Update(Candle candle, bool recalculateStats = true);

    public Candle[] Get(Guid instrumentId);

    public int GetCount(Guid instrumentId);

    public PriceStats? GetStats(Guid instrumentId);
}
