namespace Vertr.OrderExecution.Contracts.Commands;

public abstract class OrderCommandBase
{
    public Guid RequestId { get; init; }

    public Guid InstrumentId { get; init; }

    public Guid SubAccountId { get; init; }

    public DateTime CreatedAt { get; init; }
}
