using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.Application;
public class MarketDataSettings
{
    public Dictionary<Guid, CandleInterval> Subscriptions { get; set; } = [];

    public Dictionary<string, Guid> Currencies { get; set; } = [];

    public List<Guid> Instruments { get; set; } = [];
}
