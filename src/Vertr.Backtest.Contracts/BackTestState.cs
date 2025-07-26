namespace Vertr.Backtest.Contracts;
public enum BackTestState
{
    Initial = 0,
    Started = 1,
    Cancelled = 2,
    Completed = 3,
    Failed = 4,
}
