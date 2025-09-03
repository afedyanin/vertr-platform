using Vertr.MarketData.Contracts;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.Platform.BlazorUI.Components.Models;

public class PositionModel
{
    public required Position Position { get; set; }

    public required Instrument Instrument { get; set; }

    // TODO: Move it to instrument extensions
    public bool TradingDisabled =>
        string.IsNullOrEmpty(Instrument.InstrumentType) ||
        Instrument.InstrumentType.Equals("currency", StringComparison.OrdinalIgnoreCase) ||
        Position.Balance == decimal.Zero;
}
