using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Extensions;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.Platform.BlazorUI.Components.Models;

public class PositionModel
{
    public required Position Position { get; set; }

    public required Instrument Instrument { get; set; }

    public bool TradingDisabled =>
        Instrument.IsCurrency() ||
        Position.Balance == decimal.Zero;
}
