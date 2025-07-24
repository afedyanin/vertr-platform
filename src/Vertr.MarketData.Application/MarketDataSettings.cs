using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.Application;
public class MarketDataSettings
{
    public Dictionary<string, Guid> Currencies { get; set; } = [];

    public List<Guid> Instruments { get; set; } = [];

    public CandleInterval CandleInterval { get; set; } = CandleInterval.Min_1;
}
