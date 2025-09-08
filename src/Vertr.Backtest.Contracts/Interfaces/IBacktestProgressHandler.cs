namespace Vertr.Backtest.Contracts.Interfaces;
public interface IBacktestProgressHandler
{
    public Task HandleProgress(BacktestRun backtest);
}
