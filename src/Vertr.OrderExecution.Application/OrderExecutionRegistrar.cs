using Microsoft.Extensions.DependencyInjection;
using Vertr.Infrastructure.Common.Channels;
using Vertr.Infrastructure.Common.Mediator;
using Vertr.OrderExecution.Application.Services;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Interfaces;
using Vertr.PortfolioManager.Contracts;
using Vertr.Strategies.Contracts;

namespace Vertr.OrderExecution.Application;

public static class OrderExecutionRegistrar
{
    public static IServiceCollection AddOrderExecution(this IServiceCollection services)
    {
        services.RegisterDataChannel<OrderState>();
        services.RegisterDataChannel<OrderTrades>();
        services.RegisterDataChannel<TradingSignal>();
        services.RegisterDataChannel<TradeOperation>();

        services.AddHostedService<OrderStateConsumerService>();
        services.AddHostedService<OrderTradesConsumerService>();
        services.AddHostedService<TradingSignalConsumerService>();
        services.AddOptions<OrderExecutionSettings>().BindConfiguration(nameof(OrderExecutionSettings));

        services.AddMediatorHandlers(typeof(OrderExecutionRegistrar).Assembly);
        services.AddTransient<IOrderExecutionSimulator, OrderExecutionSimulator>();

        return services;
    }
}
