namespace Vertr.Backtest.Contracts;

public class BacktestRun
{
    public Guid Id { get; set; }

    public DateTime From { get; set; }

    public DateTime To { get; set; }

    public string? Description { get; set; }

    public Guid StrategyId { get; set; }

    public Guid PortfolioId { get; set; }

    public ExecutionState ExecutionState { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? ProgressMessage { get; set; }

    public bool IsCancellationRequested { get; set; }
}
