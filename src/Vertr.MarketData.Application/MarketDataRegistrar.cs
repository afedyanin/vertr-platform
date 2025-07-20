using Microsoft.Extensions.DependencyInjection;
using Vertr.MarketData.Application.Repositories;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.MarketData.Application;

public static class MarketDataRegistrar
{
    public static IServiceCollection AddMarketData(this IServiceCollection services)
    {
        services.AddOptions<MarketDataSettings>().BindConfiguration(nameof(MarketDataSettings));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MarketDataSettings).Assembly));
        services.AddSingleton<IStaticMarketDataProvider, StaticMarketDataProvider>();

        services.AddSingleton<MarketDataFeed>(); 
        services.AddSingleton<IMarketDataPublisher>(x => x.GetRequiredService<MarketDataFeed>());
        services.AddSingleton<IMarketDataConsumer>(x => x.GetRequiredService<MarketDataFeed>());

        services.AddSingleton<IMarketDataRepository, MarketDataRepository>();

        return services;
    }
}
