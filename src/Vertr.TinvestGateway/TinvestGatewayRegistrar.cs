using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tinkoff.InvestApi;
using Vertr.TinvestGateway.Abstractions;
using Vertr.TinvestGateway.Proxy;
using Vertr.TinvestGateway.Services;

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

        services.AddTransient<IInstrumentProvider, InstrumentProvider>();
        services.AddTransient<IPortfolioService, PortfolioService>();

        return services;
    }
}