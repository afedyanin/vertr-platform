namespace Vertr.OrderExecution.Application.Entities;

public class OrderEvent
{
    public Guid Id { get; set; }

    public Guid? RequestId { get; set; }

    public string? OrderId { get; set; }

    public Guid? InstrumentId { get; set; }

    public string? AccountId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? JsonData { get; set; }

    public string? JsonDataType { get; set; }
}
