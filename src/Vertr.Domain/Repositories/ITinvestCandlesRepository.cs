using Vertr.Domain.Enums;

namespace Vertr.Domain.Repositories;

public interface ITinvestCandlesRepository
{
    Task<IEnumerable<HistoricCandle>> Get(
        string symbol,
        CandleInterval interval,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<HistoricCandle>> GetLast(
        string symbol,
        CandleInterval interval,
        int count = 10,
        bool completedOnly = true,
        CancellationToken cancellationToken = default);

    Task<int> Insert(IEnumerable<HistoricCandle> candles);

    Task<int> Delete(
        string symbol,
        CandleInterval interval,
        DateTime from,
        DateTime to);
}
