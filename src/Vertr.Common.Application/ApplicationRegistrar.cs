using Microsoft.Extensions.DependencyInjection;
using Refit;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.EventHandlers;
using Vertr.Common.Application.Gateways;
using Vertr.Common.Application.LocalStorage;
using Vertr.Common.Application.Services;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application;

public static class ApplicationRegistrar
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<MarketDataPredictor>();
        services.AddSingleton<TradingSignalsGenerator>();
        services.AddSingleton<PortfolioPositionHandler>();
        services.AddSingleton<OrderExecutionHandler>();
        services.AddTransient<ICandleProcessingPipeline, CandleProcessingPipeline>();

        services.AddSingleton<IPortfoliosLocalStorage, PortfoliosLocalStorage>();
        services.AddSingleton<IInstrumentsLocalStorage, InstrumentsLocalStorage>();
        services.AddSingleton<IPortfolioManager, PortfolioManager>();

        services.AddSingleton<CandlesLocalStorage>();
        services.AddSingleton<ICandlesLocalStorage>(sp => sp.GetRequiredService<CandlesLocalStorage>());

        // TODO: Implement this
        services.AddSingleton<IPredictorGateway, PredictorGatewayStub>();

        return services;
    }

    public static IServiceCollection AddTinvestGateway(this IServiceCollection services, string baseAddress)
    {
        services.AddOrderBookQuoteProvider();
        services.AddSingleton<ITradingGateway, TinvestGateway>();

        services
           .AddRefitClient<ITinvestGatewayClient>(
               new RefitSettings
               {
                   ContentSerializer = new SystemTextJsonContentSerializer(JsonOptions.DefaultOptions)
               })
           .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseAddress));

        return services;
    }
    private static IServiceCollection AddOrderBookQuoteProvider(this IServiceCollection services)
    {
        services.AddSingleton<OrderBooksLocalStorage>();
        services.AddSingleton<IOrderBooksLocalStorage>(sp => sp.GetRequiredService<OrderBooksLocalStorage>());
        services.AddSingleton<IMarketQuoteProvider>(sp => sp.GetRequiredService<OrderBooksLocalStorage>());
        return services;
    }


    public static IServiceCollection AddBacktest(this IServiceCollection services)
    {
        services.AddSingleton<ITradingGateway, BacktestGateway>();
        services.AddSingleton<IMarketQuoteProvider>(sp => sp.GetRequiredService<CandlesLocalStorage>());

        return services;
    }
}
