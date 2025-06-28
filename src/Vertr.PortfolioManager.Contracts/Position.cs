namespace Vertr.PortfolioManager.Contracts;

public class Position
{
    public Guid InstrumentId { get; set; }

    public decimal Balance { get; set; }
}
