using Vertr.TinvestGateway.Abstractions;
using Vertr.TinvestGateway.Contracts.MarketData;
using Vertr.TinvestGateway.Repositories;

namespace Vertr.TinvestGateway.Services;

internal class InstrumentProvider : IInstrumentProvider
{
    private readonly IInstrumentRepository _instrumentRepository;

    private readonly Lazy<Task<Dictionary<Guid, Instrument>>> _instruments;

    public InstrumentProvider(IInstrumentRepository instrumentRepository)
    {
        _instrumentRepository = instrumentRepository;
        _instruments = new(LoadData());
    }

    public async ValueTask<IEnumerable<Instrument>> GetAll()
    {
        var dict = await _instruments.Value;

        if (dict == null)
        {
            return [];
        }

        return dict.Values;
    }

    public async ValueTask<Instrument?> GetById(Guid instrumentId)
    {
        var dict = await _instruments.Value;

        if (dict == null)
        {
            return null;
        }

        dict.TryGetValue(instrumentId, out var instrument);

        return instrument;
    }

    private async Task<Dictionary<Guid, Instrument>> LoadData()
    {
        var items = await _instrumentRepository.GetAll();
        return items.ToDictionary(x => x.Id, x => x);
    }
}