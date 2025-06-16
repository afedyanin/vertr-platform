using Vertr.OrderExecution.Contracts.Enums;

namespace Vertr.OrderExecution.Contracts;
public record class OrderOperation
{
    public Guid Id { get; init; }

    public DateTime CreatedAt { get; init; }

    public OperationType OperationType { get; init; }

    public string? OrderId { get; init; }

    public required string AccountId { get; init; }

    public Guid? BookId { get; init; }

    public Guid? InstrumentId { get; init; }

    public Trade[] Trades { get; init; } = [];

    public decimal? Amount { get; init; }

    public string? Message { get; init; }
}
