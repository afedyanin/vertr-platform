using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tinkoff.InvestApi;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Interfaces;
using Vertr.PortfolioManager.Contracts.Interfaces;
using Vertr.TinvestGateway.Application.BackgroundServices;
using Vertr.TinvestGateway.Application.Proxy;
using Vertr.TinvestGateway.Application.Settings;

namespace Vertr.TinvestGateway.Application;

public static class TinvestGatewayRegistrar
{
    public static IServiceCollection AddTinvestGateway(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<TinvestSettings>().BindConfiguration(nameof(TinvestSettings));
        services.AddInvestApiClient((_, settings) => configuration.Bind($"{nameof(TinvestSettings)}:{nameof(InvestApiSettings)}", settings));

        services.AddTransient<IMarketDataGateway, TinvestGatewayMarketData>();

        services.AddTransient<IPortfolioGateway, TinvestGatewayPortfolio>();
        services.AddTransient<IOrderExecutionGateway, TinvestGatewayOrders>();

        services.AddHostedService<MarketDataStreamService>();
        services.AddHostedService<OrderTradesStreamService>();

        return services;
    }
}
