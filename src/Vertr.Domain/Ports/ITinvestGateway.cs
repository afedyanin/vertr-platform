namespace Vertr.Domain.Ports;
public interface ITinvestGateway
{
    Task GetInstrument();

    Task<IEnumerable<Instrument>> FindInstrument(string query);

    Task GetCandles();
}
