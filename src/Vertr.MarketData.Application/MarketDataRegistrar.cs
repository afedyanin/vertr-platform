using Microsoft.Extensions.DependencyInjection;
using Vertr.MarketData.Application.Repositories;
using Vertr.MarketData.Application.Services;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.Platform.Common.Channels;

namespace Vertr.MarketData.Application;

public static class MarketDataRegistrar
{
    public static IServiceCollection AddMarketData(this IServiceCollection services)
    {
        services.AddOptions<MarketDataSettings>().BindConfiguration(nameof(MarketDataSettings));
        services.AddSingleton<IStaticMarketDataProvider, StaticMarketDataProvider>();
        services.AddSingleton<IMarketDataRepository, MarketDataRepository>();

        services.RegisterDataChannel<Candle>();
        services.AddHostedService<CandlesConsumerService>();

        return services;
    }
}
