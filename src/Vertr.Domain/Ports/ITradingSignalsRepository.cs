namespace Vertr.Domain.Ports;

public interface ITradingSignalsRepository
{
    Task<IEnumerable<TradingSignal>> Get(string symbol, CandleInterval interval, DateTime from, DateTime to);

    Task<IEnumerable<TradingSignal>> GetLast(string symbol, CandleInterval interval, int count = 10);

    Task<int> Insert(IEnumerable<TradingSignal> signals);

    Task<int> Delete(Guid signalId);
}
