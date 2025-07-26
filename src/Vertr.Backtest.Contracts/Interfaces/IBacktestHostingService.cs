namespace Vertr.Backtest.Contracts.Interfaces;
public interface IBacktestHostingService
{
    public Task<IBackTest[]> GetActiveBacktests();

    public Task Remove(Guid backtestId);

    public Task Start(Guid backTestId);

    public Task Cancel(Guid backtestId);

    public Task<BackTestExecutionResult?> GetResult(Guid backtestId);
}
