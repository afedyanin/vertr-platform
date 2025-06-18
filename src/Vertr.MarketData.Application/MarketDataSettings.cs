using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.Application;
public class MarketDataSettings
{
    public Dictionary<string, CandleInterval> Subscriptions { get; set; } = [];
}
