using Vertr.OrderExecution.Contracts;

namespace Vertr.PortfolioManager.Application.Entities;
public class OperationEvent
{
    public Guid Id { get; init; }

    public DateTime CreatedAt { get; init; }

    public required PortfolioIdentity PortfolioIdentity { get; init; }

    public string? JsonData { get; set; }

    public string? JsonDataType { get; set; }
}
