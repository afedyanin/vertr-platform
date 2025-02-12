namespace Vertr.Domain.Ports;
public interface ITinvestGateway
{
    Task<IEnumerable<Instrument>> FindInstrument(string query);

    Task<InstrumentDetails> GetInstrument(string ticker, string classCode);

    Task<IEnumerable<HistoricCandle>> GetCandles(
        string instrumentId,
        CandleInterval interval,
        DateTime from,
        DateTime to,
        int? limit = null);
}
