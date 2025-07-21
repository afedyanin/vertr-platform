using Microsoft.Extensions.DependencyInjection;
using Vertr.MarketData.Application.Repositories;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.MarketData.Application;

public static class MarketDataRegistrar
{
    public static IServiceCollection AddMarketData(this IServiceCollection services)
    {
        services.AddOptions<MarketDataSettings>().BindConfiguration(nameof(MarketDataSettings));
        services.AddSingleton<IStaticMarketDataProvider, StaticMarketDataProvider>();
        services.AddSingleton<IMarketDataRepository, MarketDataRepository>();

        return services;
    }
}
