using Vertr.MarketData.Contracts;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.Platform.Host.Components.Models;

public class PositionModel
{
    public required Position Position { get; set; }

    public required Instrument Instrument { get; set; }
}
