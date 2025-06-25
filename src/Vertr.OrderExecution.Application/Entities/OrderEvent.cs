using Vertr.MarketData.Contracts;
using Vertr.OrderExecution.Contracts;

namespace Vertr.OrderExecution.Application.Entities;

public class OrderEvent
{
    public Guid Id { get; set; }

    public Guid? RequestId { get; set; }

    public string? OrderId { get; set; }

    public required InstrumentIdentity InstrumentIdentity { get; set; }

    public required PortfolioIdentity PortfolioIdentity { get; init; }

    public DateTime CreatedAt { get; set; }

    public string? JsonData { get; set; }

    public string? JsonDataType { get; set; }
}
