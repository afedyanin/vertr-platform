using Vertr.MarketData.Contracts;

namespace Vertr.PortfolioManager.Contracts;

public class PortfolioPosition
{
    public required InstrumentIdentity InstrumentIdentity { get; set; }

    public decimal Balance { get; set; }
}
