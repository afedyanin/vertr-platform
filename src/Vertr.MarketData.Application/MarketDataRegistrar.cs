using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Vertr.MarketData.Application.QuartzJobs;
using Vertr.MarketData.Contracts;
using Vertr.Platform.Common.Channels;

namespace Vertr.MarketData.Application;

public static class MarketDataRegistrar
{
    public static IServiceCollection AddMarketData(this IServiceCollection services)
    {
        services.AddOptions<MarketDataSettings>().BindConfiguration(nameof(MarketDataSettings));

        services.RegisterDataChannel<Candle>();
        //services.AddHostedService<CandlesConsumerService>();

        return services;
    }

    public static IServiceCollection AddmarketDataQuartzJobs(this IServiceCollection services,
        IServiceCollectionQuartzConfigurator options)
    {
        options.AddJob<LoadIntradayCandlesJob>(LoadIntradayCandlesJobKeys.Key, j => j
               .WithDescription("Load latest candles from market data gateway"));

        options.AddTrigger(t => t
              .WithIdentity("Load intraday candles cron trigger")
              .ForJob(LoadIntradayCandlesJobKeys.Key)
              .StartAt(DateTime.UtcNow.AddMinutes(1)));

        options.AddJob<LoadHistoryCandlesJob>(LoadHistoryCandlesJobKeys.Key, j => j
               .WithDescription("Load candles history from market data gateway"));

        options.AddTrigger(t => t
              .WithIdentity("Load candles history cron trigger")
              .ForJob(LoadHistoryCandlesJobKeys.Key)
              .StartAt(DateTime.UtcNow.AddMinutes(2)));

        options.AddJob<CleanIntradayCandlesJob>(CleanIntradayCandlesJobKeys.Key, j => j
               .WithDescription("Clean old intraday candles"));

        options.AddTrigger(t => t
              .WithIdentity("Clean intraday candles cron trigger")
              .ForJob(CleanIntradayCandlesJobKeys.Key)
              .StartAt(DateTime.UtcNow.AddMinutes(3)));

        // https://www.freeformatter.com/cron-expression-generator-quartz.html
        //.WithCronSchedule("5 1/10,5/10,9/10 3-23 ? * MON-FRI"));

        return services;
    }
}
