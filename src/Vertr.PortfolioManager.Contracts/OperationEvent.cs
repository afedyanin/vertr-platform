namespace Vertr.PortfolioManager.Contracts;

public class OperationEvent
{
    public Guid Id { get; init; }

    public DateTime CreatedAt { get; init; }

    public required string AccountId { get; set; }

    public Guid? BookId { get; set; }

    public string? JsonData { get; set; }

    public string? JsonDataType { get; set; }
}
