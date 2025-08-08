using Vertr.MarketData.Contracts;
using Vertr.Strategies.Contracts;

namespace Vertr.Platform.Host.Components.Models;

public class StrategyModel
{
    public required StrategyMetadata Strategy { get; init; }

    public required Instrument Instrument { get; set; }
}
