using Microsoft.Extensions.DependencyInjection;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.PortfolioManager.Application;

public static class PortfolioManagerRegistrar
{
    public static IServiceCollection AddPortfolioManager(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PortfolioManagerRegistrar).Assembly));

        services.AddTransient<IPortfolioManager, PortfolioManager>();

        return services;
    }
}
