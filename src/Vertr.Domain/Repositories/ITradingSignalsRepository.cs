using Vertr.Domain.Settings;

namespace Vertr.Domain.Repositories;

public interface ITradingSignalsRepository
{
    public Task<TradingSignal?> GetById(
        Guid id,
        CancellationToken cancellationToken = default);

    public Task<TradingSignal?> GetLast(
        StrategySettings strategySettings,
        CancellationToken cancellationToken = default);

    public Task<int> Insert(
        TradingSignal signal,
        CancellationToken cancellationToken = default);

    public Task<int> Delete(
        Guid tradingSignalId,
        CancellationToken cancellationToken = default);
}
