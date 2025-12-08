using Vertr.Common.Contracts;

namespace Vertr.TinvestGateway.Abstractions;

public interface IInstrumentProvider
{
    public ValueTask<IEnumerable<Instrument>> GetAll();
    public ValueTask<Instrument?> GetById(Guid instrumentId);
}