namespace Vertr.Domain;

public record class PositionSnapshot
{
    public string PositionUid { get; set; }

    public DateTime TimeUtc { get; set; }

    public string AccountId { get; set; }

    public string InstrumentType { get; set; }

    public string InstrumentUid { get; set; }

    public string Symbol { get; set; }

    public decimal Quantity { get; set; }
}
