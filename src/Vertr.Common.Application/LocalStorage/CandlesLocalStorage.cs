using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.LocalStorage;

internal sealed class CandlesLocalStorage : ICandlesLocalStorage, IMarketQuoteProvider
{
    private readonly Dictionary<Guid, SortedList<DateTime, Candle>> _candles = [];

    public const int CandlesBufferLength = 100;

    public void Update(Candle candle)
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

    public Candle[] Get(Guid instrumentId)
        => _candles.TryGetValue(instrumentId, out var candleList) ? [.. candleList.Values] : [];

    public Quote? GetMarketQuote(Guid instrumentId)
    {
        var candles = Get(instrumentId);

        if (candles == null)
        {
            return null;
        }

        return GetMarketQuote(candles.Last());
    }

    private static Quote? GetMarketQuote(Candle? last)
    {
        if (last == null)
        {
            return null;
        }

        var prices = new decimal[]
        {
            last.Open,
            last.Close,
            last.High,
            last.Low
        };

        return new Quote
        {
            Time = last.TimeUtc,
            Bid = prices.Min(),
            Ask = prices.Max()
        };
    }

    public void Fill(IEnumerable<Candle> candles)
    {
        foreach (var candle in candles.OrderBy(c => c.TimeUtc))
        {
            Update(candle);
        }
    }

    public bool Any(Guid instrumentId)
        => _candles.TryGetValue(instrumentId, out var candleList) ? candleList.Any() : false;
}
