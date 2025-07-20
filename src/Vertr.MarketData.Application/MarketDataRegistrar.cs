using Microsoft.Extensions.DependencyInjection;
using Vertr.MarketData.Application.Repositories;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.Platform.Common;

namespace Vertr.MarketData.Application;

public static class MarketDataRegistrar
{
    public static IServiceCollection AddMarketData(this IServiceCollection services)
    {
        services.AddOptions<MarketDataSettings>().BindConfiguration(nameof(MarketDataSettings));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MarketDataSettings).Assembly));
        services.AddSingleton<IStaticMarketDataProvider, StaticMarketDataProvider>();

        services.AddSingleton<DataChannel<Candle>>();
        services.AddSingleton<IDataProducer<Candle>>(x => x.GetRequiredService<DataChannel<Candle>>());
        services.AddSingleton<IDataConsumer<Candle>>(x => x.GetRequiredService<DataChannel<Candle>>());

        services.AddSingleton<IMarketDataRepository, MarketDataRepository>();

        return services;
    }
}
