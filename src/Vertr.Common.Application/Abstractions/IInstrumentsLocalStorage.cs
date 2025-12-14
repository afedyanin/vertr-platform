using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Abstractions;

public interface IInstrumentsLocalStorage
{
    public Task<Instrument[]> GetAll();
    public Task<Instrument?> GetById(Guid instrumentId);
}
