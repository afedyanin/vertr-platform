using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.LocalStorage;

internal sealed class InstrumentsLocalStorage : IInstrumentsLocalStorage
{
    private readonly ITradingGateway _tinvestGateway;
    private Dictionary<Guid, Instrument>? _instrumentsDict;

    public InstrumentsLocalStorage(ITradingGateway tinvestGateway)
    {
        _tinvestGateway = tinvestGateway;
    }

    public async Task<Instrument[]> GetAll()
    {
        _instrumentsDict ??= await Init();
        return [.. _instrumentsDict.Values];
    }

    public async Task<Instrument?> GetById(Guid instrumentId)
    {
        _instrumentsDict ??= await Init();
        _instrumentsDict.TryGetValue(instrumentId, out var instrument);

        return instrument;
    }

    private async Task<Dictionary<Guid, Instrument>> Init()
    {
        var instruments = await _tinvestGateway.GetAllInstruments();
        return instruments.ToDictionary(i => i.Id);
    }
}
