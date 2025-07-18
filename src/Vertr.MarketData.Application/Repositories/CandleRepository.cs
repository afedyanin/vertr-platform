using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.Application.Repositories;

internal class CandleRepository
{
    private readonly SortedList<DateTime, Candle> _sortedCandles;

    private readonly object _lockObj = new object();

    public CandleRepository(int capacity = 1000)
    {
        _sortedCandles = new SortedList<DateTime, Candle>(capacity);
    }

    public void Add(Candle candle)
    {
        // TODO: Refactor this
        lock (_lockObj)
        {
            // TODO: Check capacity - remove first item 
            _sortedCandles.Add(candle.TimeUtc, candle);
        }
    }

    public void AddRange(Candle[] candles)
    {
        // TODO: Refactor this
        lock (_lockObj)
        {
            foreach (var candle in candles)
            {
                // TODO: Check capacity - remove first item 
                _sortedCandles.Add(candle.TimeUtc, candle);
            }
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
}
