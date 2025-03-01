using Vertr.Domain.Enums;

namespace Vertr.Domain.Repositories;

public interface ITradingSignalsRepository
{
    public Task<TradingSignal?> GetById(
        Guid id,
        CancellationToken cancellationToken = default);

    public Task<TradingSignal?> GetLast(
        string symbol,
        CandleInterval interval,
        PredictorType predictorType,
        Sb3Algo algo,
        CancellationToken cancellationToken = default);

    public Task<int> Insert(
        TradingSignal signal,
        CancellationToken cancellationToken = default);

    public Task<int> Delete(
        Guid tradingSignalId,
        CancellationToken cancellationToken = default);
}
