using Microsoft.Extensions.DependencyInjection;
using Vertr.CommandLine.Application.Services;
using Vertr.CommandLine.Models.Abstracttions;
using Vertr.Common.Mediator;

namespace Vertr.CommandLine.Application;

public static class ApplicationRegistrar
{
    public static IServiceCollection AddApplication(this IServiceCollection serviceCollection)
    {
        MediatorRegistrar.AddMediatorHandlers(serviceCollection, typeof(ApplicationRegistrar).Assembly);

        serviceCollection.AddSingleton<IMarketDataService, MarketDataService>();
        serviceCollection.AddSingleton<IPortfolioService, PortfolioService>();
        serviceCollection.AddSingleton<IOrderExecutionService, SimulatedOrderExecutionService>();

        return serviceCollection;
    }

    public static IServiceCollection AddSimulatedPredictionService(this IServiceCollection services, string predictorBaseUrl)
    {
        services.AddTransient<IPredictionService, SimulatedPredictionService>();

        return services;
    }
}