namespace Vertr.PortfolioManager.Application.Entities;

public class PortfolioPosition
{
    public Guid Id { get; set; }

    public Guid PortfolioSnapshotId { get; set; }

    public Guid InstrumentId { get; set; }

    public decimal Balance { get; set; }
}
