using Microsoft.Extensions.DependencyInjection;
using Vertr.Infrastructure.Common.Channels;
using Vertr.MarketData.Contracts;
using Vertr.Strategies.Application.Services;
using Vertr.Strategies.Contracts;
using Vertr.Strategies.Contracts.Interfaces;

namespace Vertr.Strategies.Application;
public static class StrategiesRegistrar
{
    public static IServiceCollection AddStrategies(this IServiceCollection services)
    {
        services.RegisterDataChannel<Candle>();
        services.RegisterDataChannel<TradingSignal>();

        services.AddSingleton<IStrategyRepository, StrategyRepository>(serviceProvider =>
        {
            var repo = new StrategyRepository(serviceProvider);

            // TODO: make async
            repo.InitializeAsync().GetAwaiter().GetResult();
            return repo;
        });

        services.AddSingleton<IStrategyFactory, StrategyFactory>();
        services.AddHostedService<CandlesConsumerService>();

        return services;
    }
}
