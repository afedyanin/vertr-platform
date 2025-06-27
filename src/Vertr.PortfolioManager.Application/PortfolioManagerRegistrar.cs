using Microsoft.Extensions.DependencyInjection;

namespace Vertr.PortfolioManager.Application;

public static class PortfolioManagerRegistrar
{
    public static IServiceCollection AddPortfolioManager(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PortfolioManagerRegistrar).Assembly));

        return services;
    }
}
