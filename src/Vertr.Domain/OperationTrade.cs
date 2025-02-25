namespace Vertr.Domain;

public record class OperationTrade
{
    public string TradeId { get; set; }

    public DateTime DateTime { get; set; }

    public long Quantity { get; set; }

    public decimal Price { get; set; }
}
