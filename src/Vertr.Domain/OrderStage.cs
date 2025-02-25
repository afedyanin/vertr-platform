namespace Vertr.Domain;

public record class OrderStage
{
    public string TradeId { get; set; }

    public DateTime ExecutionTime { get; set; }

    public decimal Price { get; set; }

    public long Quantity { get; set; }
}
