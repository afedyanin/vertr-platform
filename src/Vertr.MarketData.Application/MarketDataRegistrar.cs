using Microsoft.Extensions.DependencyInjection;

namespace Vertr.MarketData.Application;

public static class MarketDataRegistrar
{
    public static IServiceCollection AddMarketData(this IServiceCollection services)
    {
        services.AddOptions<MarketDataSettings>().BindConfiguration(nameof(MarketDataSettings));

        return services;
    }
}
