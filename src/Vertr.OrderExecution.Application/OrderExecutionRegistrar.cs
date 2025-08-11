using Microsoft.Extensions.DependencyInjection;
using Vertr.Infrastructure.Common.Channels;
using Vertr.OrderExecution.Application.CommandHandlers;
using Vertr.OrderExecution.Application.RequestHandlers;
using Vertr.OrderExecution.Application.Services;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Commands;
using Vertr.OrderExecution.Contracts.Interfaces;
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

        services.AddTransient<IRequestHandler<ClosePositionRequest, ExecuteOrderResponse>, ClosePositionHandler>();
        services.AddTransient<IRequestHandler<ExecuteOrderRequest, ExecuteOrderResponse>, ExecuteOrderHandler>();
        services.AddTransient<IRequestHandler<OpenPositionRequest, ExecuteOrderResponse>, OpenPositionHandler>();
        services.AddTransient<IRequestHandler<ReversePositionRequest, ExecuteOrderResponse>, ReversePositionHandler>();
        services.AddTransient<IRequestHandler<TradingSignalRequest, ExecuteOrderResponse>, TradingSignalHandler>();

        return services;
    }

    public static IServiceCollection AddSimulatedOrders(this IServiceCollection services)
    {
        services.AddTransient<IOrderExecutionGateway, OrderExecutionSimulator>();
        return services;
    }
}
