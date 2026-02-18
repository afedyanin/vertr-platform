using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Abstractions;

public interface IInstrumentsLocalStorage
{
    public Instrument[] GetAll();
    public Instrument? GetById(Guid instrumentId);
    public void Load(Instrument[] instruments);
}
