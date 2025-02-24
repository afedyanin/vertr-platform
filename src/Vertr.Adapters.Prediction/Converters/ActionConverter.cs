using Vertr.Domain.Enums;

namespace Vertr.Adapters.Prediction.Converters;
internal static class ActionConverter
{
    public static TradeAction Convert(this Models.Action action)
    {
        return action switch
        {
            Models.Action.Hold => TradeAction.Hold,
            Models.Action.Sell => TradeAction.Sell,
            Models.Action.Buy => TradeAction.Buy,
            _ => throw new InvalidOperationException($"Unknown action: {action}"),
        };
    }
}
