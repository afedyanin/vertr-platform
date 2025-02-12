namespace Vertr.Domain.Ports;

public interface ITinvestCandlesRepository
{
    Task<IEnumerable<HistoricCandle>> Get(
        string symbol,
        CandleInterval interval,
        DateTime from,
        DateTime to);

    Task<IEnumerable<HistoricCandle>> GetLast(
        string symbol,
        CandleInterval interval,
        int count = 10,
        bool completedOnly = true);

    Task<int> Insert(
        string symbol,
        CandleInterval interval,
        IEnumerable<HistoricCandle> candles);

    Task<int> Delete(
        string symbol,
        CandleInterval interval,
        DateTime from,
        DateTime to);

}
