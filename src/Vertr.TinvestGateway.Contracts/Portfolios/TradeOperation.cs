namespace Vertr.TinvestGateway.Contracts.Portfolios;

public record class TradeOperation
{
    public Guid? Id { get; set; }

    public bool IsNew => Id is null;

    public DateTime CreatedAt { get; init; }

    public TradeOperationType OperationType { get; init; }

    public string? OrderId { get; init; }

    public required string AccountId { get; init; }

    public Guid PortfolioId { get; init; }

    public Guid InstrumentId { get; init; }

    public required Money Amount { get; init; }

    public string? TradeId { get; init; }

    // EF Core limit for complex types
    // https://github.com/dotnet/efcore/issues/31376
    public required Money Price { get; init; } = new Money(decimal.Zero, "");

    public long? Quantity { get; init; }
}