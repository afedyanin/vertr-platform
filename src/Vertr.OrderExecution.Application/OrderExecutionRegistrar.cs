using Microsoft.Extensions.DependencyInjection;
using Vertr.OrderExecution.Application.CommandHandlers;
using Vertr.OrderExecution.Application.RequestHandlers;
using Vertr.OrderExecution.Application.Services;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Commands;
using Vertr.OrderExecution.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.Platform.Common.Channels;
using Vertr.Platform.Common.Mediator;
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

        services.AddScoped<ICommandHandler<ClosePositionCommand, ExecuteOrderResponse>, ClosePositionHandler>();
        services.AddScoped<ICommandHandler<ExecuteOrderCommand, ExecuteOrderResponse>, ExecuteOrderHandler>();
        services.AddScoped<ICommandHandler<OpenPositionCommand, ExecuteOrderResponse>, OpenPositionHandler>();
        services.AddScoped<ICommandHandler<ReversePositionCommand, ExecuteOrderResponse>, ReversePositionHandler>();
        services.AddScoped<ICommandHandler<TradingSignalCommand, ExecuteOrderResponse>, TradingSignalHandler>();

        return services;
    }

    public static IServiceCollection AddSimulatedOrders(this IServiceCollection services)
    {
        services.AddTransient<IOrderExecutionGateway, OrderExecutionSimulator>();
        return services;
    }
}
