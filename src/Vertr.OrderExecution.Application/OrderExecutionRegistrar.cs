using Microsoft.Extensions.DependencyInjection;
using Vertr.OrderExecution.Application.Services;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Interfaces;
using Vertr.Platform.Common.Channels;
using Vertr.Strategies.Contracts;

namespace Vertr.OrderExecution.Application;

public static class OrderExecutionRegistrar
{
    public static IServiceCollection AddOrderExecution(this IServiceCollection services)
    {
        services.RegisterDataChannel<OrderState>();
        services.RegisterDataChannel<OrderTrades>();
        services.RegisterDataChannel<TradingSignal>();

        services.AddHostedService<OrderStateConsumerService>();
        services.AddHostedService<OrderTradesConsumerService>();
        services.AddHostedService<TradingSignalConsumerService>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(OrderExecutionRegistrar).Assembly));

        return services;
    }

    public static IServiceCollection AddSimulatedOrders(this IServiceCollection services)
    {
        services.AddTransient<IOrderExecutionGateway, OrderExecutionSimulator>();
        return services;
    }
}
