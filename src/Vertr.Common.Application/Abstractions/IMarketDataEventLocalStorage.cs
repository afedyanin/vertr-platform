namespace Vertr.Common.Application.Abstractions;

public interface IMarketDataEventLocalStorage
{
    public IEnumerable<IMarketDataEvent> GetAll();

    public IMarketDataEvent? GetBySequence(int sequenceId);

    public void Save(IMarketDataEvent marketDataEvent);
}
