namespace Vertr.PortfolioManager.Application.Entities;
public class OperationEvent
{
    public Guid Id { get; init; }

    public DateTime CreatedAt { get; init; }

    public required string AccountId { get; init; }

    public Guid? BookId { get; init; }

    public string? JsonData { get; set; }

    public string? JsonDataType { get; set; }
}
