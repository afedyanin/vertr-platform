using Vertr.Common.Application.Clients;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Services;

internal sealed class InstrumentRepository : IInstrumentRepository
{
    private readonly ITinvestGatewayClient _tinvestGateway;
    private Dictionary<Guid, Instrument>? _instrumentsDict;

    public InstrumentRepository(ITinvestGatewayClient tinvestGateway)
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

public interface IInstrumentRepository
{
    public Task<Instrument[]> GetAll();
    public Task<Instrument?> GetById(Guid instrumentId);
}
