using Microsoft.Extensions.DependencyInjection;
using Vertr.OrderExecution.Contracts.Interfaces;

namespace Vertr.OrderExecution.Application;

public static class OrderExecutionRegistrar
{
    public static IServiceCollection AddOrderExecution(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(OrderExecutionRegistrar).Assembly));
        return services;
    }

    public static IServiceCollection AddSimulatedOrders(this IServiceCollection services)
    {
        services.AddTransient<IOrderExecutionGateway, OrderExecutionSimulator>();
        return services;
    }
}
