using Microsoft.Extensions.DependencyInjection;
using Vertr.MarketData.Contracts;
using Vertr.Platform.Common;
using Vertr.Strategies.Application.Services;

namespace Vertr.Strategies.Application;
public static class StrategyRegistrar
{
    public static IServiceCollection AddStrategies(this IServiceCollection services)
    {
        services.RegisterDataChannel<Candle>();
        services.AddHostedService<StrategyHostingService>();

        return services;
    }
}
