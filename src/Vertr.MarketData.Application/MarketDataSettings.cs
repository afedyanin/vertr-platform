using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.Application;
public class MarketDataSettings
{
    public CandleInterval CandleInterval { get; set; } = CandleInterval.Min_1;
    public int RemoveIntradayCandlesBeforeDays { get; set; } = 5;

    public int MaxDaysForCandleHistory { get; set; } = 10;
}
