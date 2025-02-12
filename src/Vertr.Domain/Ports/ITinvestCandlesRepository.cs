namespace Vertr.Domain.Ports;

public interface ITinvestCandlesRepository
{
    Task<IEnumerable<HistoricCandle>> GetCandles(
        string symbol,
        CandleInterval interval,
        DateTime from,
        DateTime to);

    Task<IEnumerable<HistoricCandle>> GetLastCandles(
        string symbol,
        CandleInterval interval,
        int count = 10,
        bool completedOnly = true);

    Task<int> InsertCandles(
        string symbol,
        CandleInterval interval,
        IEnumerable<HistoricCandle> candles);
}
