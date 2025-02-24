using Quartz;
using Vertr.Domain;
using Vertr.Domain.Enums;
using Vertr.Server.QuartzJobs;

namespace Vertr.Server;

internal static class QuartzRegistrar
{
    // TODO: Should move it to config?
    private static readonly string _symbols = "SBER"; // "AFKS, MOEX, OZON, SBER";
    private static readonly CandleInterval _candleInterval = CandleInterval._10Min;
    private static readonly PredictorType _predictorType = PredictorType.Sb3;
    private static readonly Sb3Algo _sb3Algo = Sb3Algo.DQN;

    public static IServiceCollection ConfigureQuatrz(this IServiceCollection services, IConfiguration configuration)
    {
        // base configuration from appsettings.json
        services.Configure<QuartzOptions>(configuration.GetSection("Quartz"));

        services.Configure<QuartzOptions>(options =>
        {
            options.Scheduling.IgnoreDuplicates = false; // default: false
            options.Scheduling.OverWriteExistingData = true; // default: true
        });

        services.AddQuartz(options =>
        {
            options.UseSimpleTypeLoader();
            options.UseInMemoryStore();
            options.UseDefaultThreadPool(tp =>
            {
                tp.MaxConcurrency = 10;
            });

            options.AddJob<UpdateLastCandlesJob>(UpdateTinvestCandlesJobKeys.Key, j => j
                   .WithDescription("Load latest candles from Tinvest API")
                   .UsingJobData(UpdateTinvestCandlesJobKeys.Symbols, _symbols)
                   .UsingJobData(UpdateTinvestCandlesJobKeys.Interval, (int)_candleInterval)
               );

            options.AddTrigger(t => t
                  .WithIdentity("Tinvest update candles cron trigger")
                  .ForJob(UpdateTinvestCandlesJobKeys.Key)
                  // https://www.freeformatter.com/cron-expression-generator-quartz.html
                  .WithCronSchedule("5 1/10,5/10,9/10 * * * ?")
              );

            options.AddJob<GenerateSignalsJob>(GenerateSignalsJobKeys.Key, j => j
                   .WithDescription("Generate DQN trading signals")
                   .UsingJobData(GenerateSignalsJobKeys.Symbols, _symbols)
                   .UsingJobData(GenerateSignalsJobKeys.Interval, (int)_candleInterval)
                   .UsingJobData(GenerateSignalsJobKeys.PredictorType, _predictorType.Name)
                   .UsingJobData(GenerateSignalsJobKeys.Sb3Algo, _sb3Algo.Name)
               );

            options.AddTrigger(t => t
                  .WithIdentity("Generate DQN trading signals cron trigger")
                  .ForJob(GenerateSignalsJobKeys.Key)
                  .WithCronSchedule("10 9/10 * * * ?")
              );
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }
}
