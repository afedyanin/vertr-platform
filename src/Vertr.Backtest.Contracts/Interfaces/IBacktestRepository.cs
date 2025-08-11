namespace Vertr.Backtest.Contracts.Interfaces;

public interface IBacktestRepository
{
    public Task<BacktestRun[]> GetAll();

    public Task<BacktestRun?> GetById(Guid id);

    public Task<bool> Save(BacktestRun backtest);

    public Task<int> Delete(Guid Id);

}
