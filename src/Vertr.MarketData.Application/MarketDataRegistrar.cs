using Microsoft.Extensions.DependencyInjection;
using Vertr.Infrastructure.Common.Channels;
using Vertr.MarketData.Application.CommandHandlers;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Commands;
using Vertr.Platform.Common.Mediator;

namespace Vertr.MarketData.Application;

public static class MarketDataRegistrar
{
    public static IServiceCollection AddMarketData(this IServiceCollection services)
    {
        services.AddOptions<MarketDataSettings>().BindConfiguration(nameof(MarketDataSettings));
        services.RegisterDataChannel<Candle>();

        services.AddTransient<IRequestHandler<CleanIntradayCandlesRequest>, CleanIntradayCandlesHandler>();
        services.AddTransient<IRequestHandler<LoadHistoryCandlesRequest>, LoadHistoryCandlesHandler>();
        services.AddTransient<IRequestHandler<LoadIntradayCandlesRequest>, LoadIntradayCandlesHandler>();

        return services;
    }
}
