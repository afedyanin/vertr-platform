using Microsoft.Extensions.DependencyInjection;
using Vertr.MarketData.Contracts;
using Vertr.Platform.Common.Channels;

namespace Vertr.MarketData.Application;

public static class MarketDataRegistrar
{
    public static IServiceCollection AddMarketData(this IServiceCollection services)
    {
        services.AddOptions<MarketDataSettings>().BindConfiguration(nameof(MarketDataSettings));
        services.RegisterDataChannel<Candle>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MarketDataRegistrar).Assembly));

        return services;
    }
}
