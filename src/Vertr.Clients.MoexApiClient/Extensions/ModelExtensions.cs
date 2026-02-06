using Vertr.Clients.MoexApiClient.Models;

namespace Vertr.Clients.MoexApiClient.Extensions;

internal static class ModelExtensions
{
    public static FutureInfo? ToFutureInfo(this IEnumerable<SecurityInfoItem> items)
    {
        var dict = items.ToDictionary(item => item.Name);

        if (!dict.TryGetValue("SECID", out var tickerItem))
        {
            return null;
        }

        if (!dict.TryGetValue("SHORTNAME", out var nameItem))
        {
            return null;
        }

        if (!dict.TryGetValue("LSTDELDATE", out var expDateItem))
        {
            return null;
        }

        if (!dict.TryGetValue("LOTSIZE", out var lotSizeItem))
        {
            return null;
        }

        if (!dict.TryGetValue("UNIT", out var unitItem))
        {
            return null;
        }

        var res = new FutureInfo()
        {
            Ticker = tickerItem.Value,
            Name = nameItem.Value,
            ExpDate = DateOnly.Parse(expDateItem.Value),
            LotSize = int.Parse(lotSizeItem.Value),
            Unit = unitItem.Value,
        };

        return res;
    }

    public static IEnumerable<IndexRate> ToIndexRates(this IEnumerable<CandleItem> items, string ticker)
        => items.Select(i => i.ToIndexRate(ticker));

    public static IndexRate ToIndexRate(this CandleItem item, string ticker)
    {
        var res = new IndexRate()
        {
            Ticker = ticker,
            Time = item.End,
            Value = item.Close,
        };

        return res;
    }
}
