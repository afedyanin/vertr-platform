namespace Vertr.OrderExecution.Contracts;
public class OrderOperation
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public OperationType OperationType { get; set; }

    public string? OrderId { get; set; }

    public string? AccountId { get; set; }

    public Guid? BookId { get; set; }

    public Guid? InstrumentId { get; set; }

    public Trade[] Trades { get; set; } = [];

    public decimal? Amount { get; set; }

    public string? Message { get; set; }
}
