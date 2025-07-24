using Microsoft.Extensions.DependencyInjection;
using Vertr.Platform.Common.Channels;
using Vertr.Strategies.Application.Services;
using Vertr.Strategies.Contracts;

namespace Vertr.Strategies.Application;
public static class StrategiesRegistrar
{
    public static IServiceCollection AddStrategies(this IServiceCollection services)
    {
        services.RegisterDataChannel<StrategyMetadata>();
        services.AddHostedService<StrategyHostingService>();

        return services;
    }
}
