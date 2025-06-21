using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tinkoff.InvestApi;
using Vertr.TinvestGateway.Application.BackgroundServices;
using Vertr.TinvestGateway.Application.Proxy;
using Vertr.TinvestGateway.Application.Settings;
using Vertr.TinvestGateway.Contracts.Interfaces;

namespace Vertr.TinvestGateway.Application;

public static class TinvestGatewayRegistrar
{
    public static IServiceCollection AddTinvestGateway(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<TinvestSettings>().BindConfiguration(nameof(TinvestSettings));
        services.AddInvestApiClient((_, settings) => configuration.Bind($"{nameof(TinvestSettings)}:{nameof(InvestApiSettings)}", settings));

        services.AddTransient<ITinvestGatewayAccounts, TinvestGatewayAccounts>();
        services.AddTransient<ITinvestGatewayMarketData, TinvestGatewayMarketData>();
        services.AddTransient<ITinvestGatewayOrders, TinvestGatewayOrders>();

        //services.AddHostedService<OrderTradesStreamService>();
        //services.AddHostedService<OrderStateStreamService>();
        //services.AddHostedService<MarketDataStreamService>();
        //services.AddHostedService<PortfolioStreamService>();

        return services;
    }
}
