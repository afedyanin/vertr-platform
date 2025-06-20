using Microsoft.Extensions.DependencyInjection;

namespace Vertr.OrderExecution.Application;

public static class ApplicationRegistrar
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationRegistrar).Assembly));

        return services;
    }
}
