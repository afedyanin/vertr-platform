using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.Application;
public class MarketDataSettings
{
    public CandleInterval CandleInterval { get; set; } = CandleInterval.Min_1;
}
