using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.Application.Repositories;

internal class CandleRepository
{
    private readonly SortedList<DateTime, Candle> _sortedCandles;
    private readonly int _maxCapacity;

    public bool IsFull => _sortedCandles.Count >= _maxCapacity;

    public CandleRepository(int capacity = 1000)
    {
        _maxCapacity = capacity;
        _sortedCandles = new SortedList<DateTime, Candle>(_maxCapacity);
    }

    public void Add(Candle candle)
    {
        CheckAndAdd(candle);
    }

    public void AddRange(Candle[] candles)
    {
        foreach (var candle in candles)
        {
            CheckAndAdd(candle);
        }
    }

    public Candle? GetLast()
        => _sortedCandles.LastOrDefault().Value;

    public Candle[] GetAll(int maxCount = 0)
    {
        if (maxCount == 0)
        {
            return [.. _sortedCandles.Values];
        }

        return [.. _sortedCandles.Take(maxCount).Select(kvp => kvp.Value)];
    }

    private void CheckAndAdd(Candle candle)
    {
        if (IsFull)
        {
            // remove oldest entry
            _sortedCandles.Remove(_sortedCandles.First().Key);
        }

        _sortedCandles[candle.TimeUtc] = candle;
    }
}
