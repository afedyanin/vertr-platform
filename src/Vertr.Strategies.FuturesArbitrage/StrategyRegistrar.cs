using Microsoft.Extensions.DependencyInjection;

namespace Vertr.Strategies.FuturesArbitrage;

public static class StrategyRegistrar
{
    public static IServiceCollection AddCandlesForecastStrategy(this IServiceCollection services)
    {
        /*
        services.AddSingleton<IEventHandler<CandleReceivedEvent>, MarketDataPredictor>();
        services.AddSingleton<TradingSignalsGenerator>();
        services.AddSingleton<PortfolioPositionHandler>();
        services.AddSingleton<OrderExecutionHandler>();

        services.AddTransient<ICandleProcessingPipeline, CandleProcessingPipeline>();

        // services.AddSingleton<IPredictorGateway, PredictorGatewayStub>();
        services.AddSingleton<IPredictorGateway, PredictorGateway>();
        */
        return services;
    }

    public static IServiceCollection AddCandlesForecastBacktest(this IServiceCollection services)
    {
        /*
        services.AddSingleton<IHistoricCandlesProvider, CsvHistoricCandlesProvider>();
        services.AddSingleton<IMarketQuoteProvider>(sp => sp.GetRequiredService<CandlesLocalStorage>());
        services.AddSingleton<ITradingGateway, BacktestGateway>();
        */
        return services;
    }
}
