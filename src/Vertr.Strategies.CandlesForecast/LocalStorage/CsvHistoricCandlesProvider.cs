using Vertr.Common.Application.Extensions;
using Vertr.Common.Contracts;
using Vertr.Strategies.CandlesForecast.Abstractions;
using Vertr.Strategies.CandlesForecast.Helpers;

namespace Vertr.Strategies.CandlesForecast.LocalStorage;

internal sealed class CsvHistoricCandlesProvider : IHistoricCandlesProvider
{
    private readonly Dictionary<Guid, IEnumerable<Candle>> _candlesDict = [];

    public async Task Load(string pathToCsv, Guid instrumentId)
    {
        var candles = CsvImporter.LoadCandles(pathToCsv, instrumentId);
        _candlesDict[instrumentId] = candles.OrderBy(c => c.TimeUtc);
    }

    public CandleRangeInfo? GetRange(Guid instrumentId)
    {
        return !_candlesDict.TryGetValue(instrumentId, out var items) ? null : items.GetCandlesRange();
    }

    public IEnumerable<Candle> Get(Guid instrumentId, int skip = 0, int take = 0)
    {
        if (!_candlesDict.TryGetValue(instrumentId, out var items))
        {
            return [];
        }

        if (skip > 0)
        {
            items = items.Skip(skip);
        }

        if (take > 0)
        {
            items = items.Take(take);
        }

        return items;
    }
}
