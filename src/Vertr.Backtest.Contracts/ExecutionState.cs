namespace Vertr.Backtest.Contracts;

public enum ExecutionState
{
    Created = 0,
    Enqueued = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4,
    Failed = 5,
}
