using Vertr.MarketData.Contracts;

namespace Vertr.PortfolioManager.Contracts;

public class Position
{
    public required InstrumentIdentity InstrumentIdentity { get; set; }

    public decimal Balance { get; set; }
}
