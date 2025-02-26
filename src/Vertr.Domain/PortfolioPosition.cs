namespace Vertr.Domain;

public record class PortfolioPosition
{
    public Guid Id { get; set; }

    public Guid PortfolioSnapshotId { get; set; }

    public string InstrumentType { get; set; }

    public decimal Quantity { get; set; }

    public decimal AveragePositionPrice { get; set; }

    public decimal ExpectedYield { get; set; }

    public decimal CurrentNkd { get; set; }

    public decimal CurrentPrice { get; set; }

    public decimal AveragePositionPriceFifo { get; set; }

    public bool Blocked { get; set; }

    public decimal BlockedLots { get; set; }

    public Guid PositionUid { get; set; }

    public Guid InstrumentUid { get; set; }

    public decimal VarMargin { get; set; }

    public decimal ExpectedYieldFifo { get; set; }

    public PortfolioSnapshot PortfolioSnapshot { get; set; }
}
