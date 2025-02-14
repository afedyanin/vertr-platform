using Quartz;
using Vertr.Domain;
using Vertr.Server.QuartzJobs;

namespace Vertr.Server;

internal static class QuartzRegistrar
{
    // https://github.dev/246850/Calamus.TaskScheduler/blob/00b1c30c21bf23eaa5cd8497359f39f930562d7d/Calamus.TaskScheduler/Startup.cs#L34#L154
    // https://www.quartz-scheduler.net/documentation/quartz-3.x/packages/microsoft-di-integration.html#persistent-job-stores

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

            options.AddJob<UpdateLastCandlesJob>(LoadTinvestCandlesJobKeys.Key, j => j
                   .WithDescription("Load latest candles from Tinvest API")
                   // TODO: Should move it to jobOptions?
                   // .UsingJobData(LoadTinvestCandlesJobKeys.Symbols, "SBER")
                   .UsingJobData(LoadTinvestCandlesJobKeys.Symbols, "AFKS, MOEX, OZON, SBER")
                   .UsingJobData(LoadTinvestCandlesJobKeys.Interval, (int)CandleInterval.Min10)
               );

            options.AddTrigger(t => t
                  .WithIdentity("Tinvest update candles cron trigger")
                  .ForJob(LoadTinvestCandlesJobKeys.Key)
                  .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(3)))
                  // TODO: Should move it to jobOptions?
                  // https://www.freeformatter.com/cron-expression-generator-quartz.html
                  .WithCronSchedule("5 1/10,5/10,9/10 * * * ?")
              );
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }
}
