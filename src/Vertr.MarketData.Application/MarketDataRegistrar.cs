using Microsoft.Extensions.DependencyInjection;

namespace Vertr.MarketData.Application;

public static class MarketDataRegistrar
{
    public static IServiceCollection AddMarketData(this IServiceCollection services)
    {
        services.AddOptions<MarketDataSettings>().BindConfiguration(nameof(MarketDataSettings));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MarketDataSettings).Assembly));

        return services;
    }
}
