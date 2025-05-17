using Microsoft.Extensions.DependencyInjection;
using Vertr.OrderExecution.Application.Abstractions;
using Vertr.OrderExecution.Application.Services;

namespace Vertr.OrderExecution.Application;

public static class ApplicationRegistrar
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationRegistrar).Assembly));
        services.AddScoped<IOrderExecutionService, TinvestOrderExecutionService>();
        services.AddScoped<IStaticMarketDataProvider, StaticMarketDataProvider>();

        return services;
    }
}
