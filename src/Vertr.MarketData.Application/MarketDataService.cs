using Microsoft.Extensions.Options;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.MarketData.Application;

internal class MarketDataService : IMarketDataService
{
    private readonly MarketDataSettings _settings;

    public MarketDataService(
        IOptions<MarketDataSettings> options
        )
    {
        _settings = options.Value;
    }

    public Task<CandleSubscription[]> GetSubscriptions()
        => Task.FromResult(_settings.GetCandleSubscriptions());
}
