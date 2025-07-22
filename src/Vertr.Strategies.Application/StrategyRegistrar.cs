using Microsoft.Extensions.DependencyInjection;
using Vertr.MarketData.Contracts;
using Vertr.Platform.Common.Channels;
using Vertr.Strategies.Application.Services;

namespace Vertr.Strategies.Application;
public static class StrategyRegistrar
{
    public static IServiceCollection AddStrategies(this IServiceCollection services)
    {
        services.RegisterDataChannel<Candle>();
        services.AddHostedService<CandlesConsumerService>();

        return services;
    }
}
