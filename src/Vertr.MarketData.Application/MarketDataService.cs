using Microsoft.Extensions.Options;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.TinvestGateway.Contracts.Interfaces;

namespace Vertr.MarketData.Application;

internal class MarketDataService : IMarketDataService
{
    private readonly MarketDataSettings _settings;
    private readonly ITinvestGatewayMarketData _tinvestGatewayMarketData;

    public MarketDataService(
        IOptions<MarketDataSettings> options,
        ITinvestGatewayMarketData tinvestGatewayMarketData
        )
    {
        _settings = options.Value;
        _tinvestGatewayMarketData = tinvestGatewayMarketData;
    }

    public Task<CandleSubscription[]> GetSubscriptions()
        => Task.FromResult(_settings.GetCandleSubscriptions());

    public async Task<Instrument?> GetInstrument(string instrumentId)
    {
        // TODO: Use Redis to Cache instrument
        var instrument = await _tinvestGatewayMarketData.GetInstrumentById(instrumentId);
        return instrument;
    }
}
