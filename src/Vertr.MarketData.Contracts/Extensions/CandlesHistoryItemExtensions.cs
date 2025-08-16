using System.Text;
using System.Text.Json;

namespace Vertr.MarketData.Contracts.Extensions;
public static class CandlesHistoryItemExtensions
{
    public static Candle[] GetCandles(this CandlesHistoryItem? item)
    {
        if (item == null)
        {
            return [];
        }

        var json = Encoding.UTF8.GetString(item.Data);

        var candles = JsonSerializer.Deserialize<Candle[]>(json,
            Platform.Common.Utils.JsonOptions.DefaultOptions);

        return candles?.ToArray() ?? [];
    }
}
