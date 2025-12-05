using Vertr.TinvestGateway.Contracts.MarketData;

namespace Vertr.TinvestGateway.Abstractions;

public interface IInstrumentProvider
{
    public ValueTask<IEnumerable<Instrument>> GetAll();
    public ValueTask<Instrument?> GetById(Guid instrumentId);
}