namespace Vertr.Strategies.Contracts.Interfaces;

public interface ITradingSignalRepository
{
    public Task<TradingSignal[]> GetByStrategyId(Guid strategyId);

    public Task<TradingSignal?> GetById(Guid id);

    public Task<bool> Save(TradingSignal signal);

    public Task<int> Delete(Guid Id);
}
