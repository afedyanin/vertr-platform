using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.Application;
public class MarketDataSettings
{
    public Dictionary<string, CandleInterval> Subscriptions { get; set; } = [];

    public List<Guid> Instruments { get; set; } = [];

    public CandleSubscription[] GetCandleSubscriptions()
    {
        var res = new List<CandleSubscription>();

        foreach (var kvp in Subscriptions)
        {
            (var clasCcode, var ticker) = ParseKey(kvp.Key);

            if (clasCcode == null || ticker == null)
            {
                continue;
            }

            res.Add(new CandleSubscription
            {
                InstrumentIdentity = new InstrumentIdentity(clasCcode, ticker),
                Interval = kvp.Value
            });

        }

        return [.. res];
    }

    private static (string?, string?) ParseKey(string key)
    {
        var items = key.Split('.');

        if (items != null && items.Length == 2)
        {
            return (items[0], items[1]);
        }

        return (null, null);
    }
}
