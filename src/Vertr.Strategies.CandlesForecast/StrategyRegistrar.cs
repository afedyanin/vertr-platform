using Microsoft.Extensions.DependencyInjection;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.LocalStorage;
using Vertr.Strategies.CandlesForecast.Abstractions;
using Vertr.Strategies.CandlesForecast.EventHandlers;
using Vertr.Strategies.CandlesForecast.Gateways;
using Vertr.Strategies.CandlesForecast.LocalStorage;
using Vertr.Strategies.CandlesForecast.Services;

namespace Vertr.Strategies.CandlesForecast;

public static class StrategyRegistrar
{
    public static IServiceCollection AddCandlesForecastStrategy(this IServiceCollection services)
    {
        services.AddSingleton<MarketDataPredictor>();
        services.AddSingleton<TradingSignalsGenerator>();
        services.AddSingleton<PortfolioPositionHandler>();
        services.AddSingleton<OrderExecutionHandler>();

        services.AddSingleton<ICandleProcessingPipeline, CandleProcessingPipeline>();

        // services.AddSingleton<IPredictorGateway, PredictorGatewayStub>();
        services.AddSingleton<IPredictorGateway, PredictorGateway>();

        return services;
    }

    public static IServiceCollection AddCandlesForecastBacktest(this IServiceCollection services)
    {
        services.AddSingleton<IHistoricCandlesProvider, CsvHistoricCandlesProvider>();
        services.AddSingleton<IMarketQuoteProvider>(sp => sp.GetRequiredService<CandlesLocalStorage>());
        services.AddSingleton<ITradingGateway, BacktestGateway>();

        return services;
    }
}
