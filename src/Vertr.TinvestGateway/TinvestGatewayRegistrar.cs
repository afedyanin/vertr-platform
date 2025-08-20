using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tinkoff.InvestApi;
using Vertr.Infrastructure.Common.Channels;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Interfaces;
using Vertr.PortfolioManager.Contracts.Interfaces;
using Vertr.TinvestGateway.BackgroundServices;
using Vertr.TinvestGateway.Proxy;

namespace Vertr.TinvestGateway;

public static class TinvestGatewayRegistrar
{
    public static IServiceCollection AddTinvestGateways(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<TinvestSettings>().BindConfiguration(nameof(TinvestSettings));
        services.AddInvestApiClient((_, settings) => configuration.Bind($"{nameof(TinvestSettings)}:{nameof(InvestApiSettings)}", settings));

        services.AddTransient<IMarketDataGateway, TinvestGatewayMarketData>();
        services.AddTransient<IPortfolioGateway, TinvestGatewayPortfolio>();
        services.AddTransient<IOrderExecutionGateway, TinvestGatewayOrders>();

        return services;
    }

    public static IServiceCollection AddTinvestStreams(this IServiceCollection services)
    {
        services.RegisterDataChannel<Candle>();
        services.RegisterDataChannel<OrderState>();
        services.RegisterDataChannel<OrderTrades>();

        services.AddHostedService<MarketDataStreamService>();
        services.AddHostedService<OrderTradesStreamService>();
        services.AddHostedService<OrderStateStreamService>();

        return services;
    }
}
