namespace Vertr.OrderExecution.Contracts.Commands;

public abstract class OrderCommandBase
{
    public Guid RequestId { get; init; }

    public Guid InstrumentId { get; init; }

    public Guid PortfolioId { get; init; }

    public DateTime CreatedAt { get; init; }

    public Guid? BacktestId { get; init; }

    public decimal Price { get; init; }
}
