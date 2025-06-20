using Microsoft.Extensions.DependencyInjection;
using Vertr.TinvestGateway.Application.BackgroundServices;
using Vertr.TinvestGateway.Application.Proxy;
using Vertr.TinvestGateway.Application.Settings;
using Vertr.TinvestGateway.Contracts.Interfaces;

namespace Vertr.TinvestGateway.Application;

public static class TinvestGatewayRegistrar
{
    public static IServiceCollection AddTinvestGateway(this IServiceCollection services)
    {
        services.AddTransient<ITinvestGatewayAccounts, TinvestGatewayAccounts>();
        services.AddTransient<ITinvestGatewayMarketData, TinvestGatewayMarketData>();
        services.AddTransient<ITinvestGatewayOrders, TinvestGatewayOrders>();
        services.AddTransient<ITinvestGatewayPositions, TinvestGatewayPositions>();

        services.AddHostedService<OrderTradesStreamService>();
        services.AddHostedService<OrderStateStreamService>();
        services.AddHostedService<MarketDataStreamService>();
        services.AddHostedService<PortfolioStreamService>();
        services.AddHostedService<PositionStreamService>();

        services.AddOptions<TinvestSettings>().BindConfiguration(nameof(TinvestSettings));


        return services;
    }
}
