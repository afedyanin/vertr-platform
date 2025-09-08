namespace Vertr.Backtest.Contracts.Interfaces;

public interface IBacktestRepository
{
    public Task<BacktestRun[]> GetAll();

    public Task<BacktestRun?> GetById(Guid id);

    public Task<BacktestRun?> GetByPortfolioId(Guid portfolioId);

    public Task<bool> Save(BacktestRun backtest);

    public Task<bool> Cancel(Guid id);

    public Task<int> Delete(Guid Id);

}
