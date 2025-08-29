using Vertr.MarketData.Contracts;
using Vertr.PortfolioManager.Contracts;
using Vertr.Strategies.Contracts;

namespace Vertr.Platform.Host.Components.Models;

public class StrategyModel
{
    public required StrategyMetadata Strategy { get; init; }

    public required Instrument Instrument { get; set; }

    public required Portfolio Portfolio { get; set; }
}
