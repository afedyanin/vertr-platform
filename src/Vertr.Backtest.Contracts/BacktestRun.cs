namespace Vertr.Backtest.Contracts;

public class BacktestRun
{
    public Guid Id { get; set; }

    public DateTime From { get; set; }

    public DateTime To { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? Description { get; set; }

    public Guid StrategyId { get; set; }

    public Guid InsrumentId { get; set; }

    public Guid SubAccountId { get; set; }

    public ExecutionState ExecutionState { get; set; }

    public DateTime? ProgressTime { get; set; }

    public string? ProgressMessage { get; set; }

    public bool IsCancellationRequested { get; set; }
}
