using Microsoft.Extensions.DependencyInjection;
using Vertr.Infrastructure.Common.Channels;
using Vertr.Infrastructure.Common.Mediator;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.MarketData.Application;

public static class MarketDataRegistrar
{
    public static IServiceCollection AddMarketData(this IServiceCollection services)
    {
        services.AddOptions<MarketDataSettings>().BindConfiguration(nameof(MarketDataSettings));

        services.RegisterDataChannel<Candle>();
        services.RegisterDataChannel<CandleSubscription>();

        services.AddMediatorHandlers(typeof(MarketDataRegistrar).Assembly);
        services.AddSingleton<ICandlesHistoryLoader, CandlesHistoryLoader>();
        return services;
    }
}
