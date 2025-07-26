namespace Vertr.Backtest.Contracts.Interfaces;
public interface IBackTest
{
    public Guid Id { get; }

    public Task Start();

    public Task Stop();

    public Task<BackTestExecutionResult> GetResult();
}
