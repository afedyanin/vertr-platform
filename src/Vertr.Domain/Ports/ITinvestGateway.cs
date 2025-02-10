namespace Vertr.Domain.Ports;
public interface ITinvestGateway
{
    Task GetInstrument(string ticker, string classCode);

    Task<IEnumerable<Instrument>> FindInstrument(string query);

    Task GetCandles();
}
