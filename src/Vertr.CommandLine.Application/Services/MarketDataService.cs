using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.Abstracttions;
using Vertr.CommandLine.Models.Helpers;

namespace Vertr.CommandLine.Application.Services;

internal class MarketDataService : IMarketDataService
{
    private readonly Dictionary<string, Candle[]> _storage = [];

    public Task<IEnumerable<DateTime>> GetEnumerable(string symbol)
    {
        _storage.TryGetValue(symbol, out var candles);
        var enumerable = candles.GetTimeEnumerable();

        return Task.FromResult(enumerable);
    }
    public Task<Candle[]> GetCandles(string symbol, DateTime before, int count = 1)
    {
        Candle[] res = [];

        if (_storage.TryGetValue(symbol, out var candles))
        {
            res = [.. candles.GetEqualOrLessThanBefore(before, count)];
        }

        return Task.FromResult(res);
    }

    public Task<decimal?> GetMarketPrice(string symbol, DateTime time, PriceType priceType, int shift = 0)
    {
        if (_storage.TryGetValue(symbol, out var candles))
        {
            var candle = candles.GetShifted(time, shift);
            var price = candle.GetPrice(priceType);
            return Task.FromResult(price);
        }

        return Task.FromResult<decimal?>(null);
    }

    public Task LoadData(string symbol, IEnumerable<Candle> candles)
    {
        _storage[symbol] = [.. candles.OrderBy(c => c.TimeUtc)];
        return Task.CompletedTask;
    }

    public Task<CandleRange?> GetCandleRange(string symbol)
    {
        _storage.TryGetValue(symbol, out var candles);
        var range = candles.GetRange(symbol);

        return Task.FromResult(range);
    }

    public Task<Dictionary<DateOnly, CandleRange>> GetCandleRanges(string symbol)
    {
        _storage.TryGetValue(symbol, out var candles);
        var range = candles.GetRanges(symbol);

        return Task.FromResult(range);
    }
}