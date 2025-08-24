using Vertr.PortfolioManager.Contracts;

namespace Vertr.Platform.Host.Components.Models;

public class TradeOperationModel
{
    public DateTime CreatedAt { get; init; }

    public required string OperationType { get; init; }

    public required string Instrument { get; init; }

    public decimal Amount { get; init; }

    public required string Currency { get; init; }

    public decimal? Price { get; init; }

    public decimal? Quantity { get; init; }

    public string? OrderId { get; init; }
}
