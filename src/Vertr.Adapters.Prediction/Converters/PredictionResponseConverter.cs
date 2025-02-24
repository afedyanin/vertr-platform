using Vertr.Adapters.Prediction.Models;
using Vertr.Domain.Enums;

namespace Vertr.Adapters.Prediction.Converters;
internal static class PredictionResponseConverter
{
    public static IEnumerable<(DateTime, TradeAction)> Convert(this PredictionResponse response)
    {
        var items = new List<(DateTime, TradeAction)>();

        for (int i = 0; i < response.Time.Length; i++)
        {
            items.Add((response.Time[i], response.Action[i].Convert()));
        }

        return items;
    }
}
