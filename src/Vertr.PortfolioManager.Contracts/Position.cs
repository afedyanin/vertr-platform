namespace Vertr.PortfolioManager.Contracts;

public class Position
{
    public Guid Id { get; set; }

    public Guid PortfolioId { get; set; }

    public Guid InstrumentId { get; set; }

    public decimal Balance { get; set; }
}
