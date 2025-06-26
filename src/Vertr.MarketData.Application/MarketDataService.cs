using Microsoft.Extensions.Options;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.TinvestGateway.Contracts.Interfaces;

namespace Vertr.MarketData.Application;

internal class MarketDataService : IMarketDataService
{
    private readonly MarketDataSettings _settings;
    private readonly ITinvestGatewayMarketData _tinvestGatewayMarketData;
    private readonly IStaticMarketDataRepository _marketInstrumentRepository;

    public MarketDataService(
        IOptions<MarketDataSettings> options,
        ITinvestGatewayMarketData tinvestGatewayMarketData,
        IStaticMarketDataRepository marketInstrumentRepository)
    {
        _settings = options.Value;
        _tinvestGatewayMarketData = tinvestGatewayMarketData;
        _marketInstrumentRepository = marketInstrumentRepository;
    }

    public async Task Initialize()
    {
        await _marketInstrumentRepository.Clear();

        // TODO: Add static instruments from config (RUB)
        var instruments = new List<Instrument>();

        var subscriptions = _settings.GetCandleSubscriptions();

        foreach (var subscription in subscriptions)
        {
            var instrument = await _tinvestGatewayMarketData.GetInstrument(subscription.InstrumentIdentity);
            var interval = subscription.Interval;

            if (instrument != null)
            {
                instruments.Add(instrument);
            }
        }

        await _marketInstrumentRepository.Save([.. instruments]);
    }

    public Task<Instrument?> GetInstrument(InstrumentIdentity instrumentIdentity)
        => _marketInstrumentRepository.Get(instrumentIdentity);

    public Task<CandleSubscription[]?> GetSubscriptions()
    {
        throw new NotImplementedException();
    }
}
