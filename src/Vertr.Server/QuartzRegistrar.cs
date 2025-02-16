using Quartz;
using Vertr.Domain;
using Vertr.Server.QuartzJobs;

namespace Vertr.Server;

internal static class QuartzRegistrar
{
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
                   // TODO: Should move it to jobOptions?
                   // .UsingJobData(LoadTinvestCandlesJobKeys.Symbols, "SBER")
                   .UsingJobData(UpdateTinvestCandlesJobKeys.Symbols, "AFKS, MOEX, OZON, SBER")
                   .UsingJobData(UpdateTinvestCandlesJobKeys.Interval, (int)CandleInterval.Min10)
               );

            options.AddTrigger(t => t
                  .WithIdentity("Tinvest update candles cron trigger")
                  .ForJob(UpdateTinvestCandlesJobKeys.Key)
                  .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(3)))
                  // TODO: Should move it to jobOptions?
                  // https://www.freeformatter.com/cron-expression-generator-quartz.html
                  .WithCronSchedule("5 1/10,5/10,9/10 * * * ?")
              );

            options.AddJob<GenerateSignalsJob>(GenerateSignalsJobKeys.Key, j => j
                   .WithDescription("Generate trading signals from last candles")
                   // TODO: Should move it to jobOptions?
                   // .UsingJobData(LoadTinvestCandlesJobKeys.Symbols, "SBER")
                   .UsingJobData(GenerateSignalsJobKeys.Symbols, "AFKS, MOEX, OZON, SBER")
                   .UsingJobData(GenerateSignalsJobKeys.Interval, (int)CandleInterval.Min10)
               );

            options.AddTrigger(t => t
                  .WithIdentity("Generate trading signals cron trigger")
                  .ForJob(GenerateSignalsJobKeys.Key)
                  .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(3)))
                  // TODO: Should move it to jobOptions?
                  // https://www.freeformatter.com/cron-expression-generator-quartz.html
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
