using Vertr.Backtest.Contracts;
using Vertr.Backtest.Contracts.Interfaces;

namespace Vertr.Backtest.Application.Services;
internal class BacktestHostingService : IBacktestHostingService
{
    private readonly IDictionary<IBackTest, Task> _backtests = new Dictionary<IBackTest, Task>();

    public Task Cancel(Guid backtestId)
    {
        throw new NotImplementedException();
    }

    public Task<IBackTest[]> GetActiveBacktests()
        => Task.FromResult(_backtests.Keys.ToArray());

    public Task<BackTestExecutionResult?> GetResult(Guid backtestId)
    {
        throw new NotImplementedException();
    }

    public Task Remove(Guid backtestId)
    {
        throw new NotImplementedException();
    }

    public Task Start(Guid backTestId)
    {
        throw new NotImplementedException();
    }
}
