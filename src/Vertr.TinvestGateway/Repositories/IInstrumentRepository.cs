using Vertr.Common.Contracts;

namespace Vertr.TinvestGateway.Repositories;

public interface IInstrumentRepository
{
    public Task Clear();
    public Task<IEnumerable<Instrument>> GetAll();
    public Task<Instrument?> Get(Guid id);
    public Task Save(Instrument instrument);
}