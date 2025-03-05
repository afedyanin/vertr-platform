using Quartz;
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
                   .WithDescription("Load latest candles from Tinvest API"));

            options.AddTrigger(t => t
                  .WithIdentity("Tinvest update candles cron trigger")
                  .ForJob(UpdateTinvestCandlesJobKeys.Key)
                  // https://www.freeformatter.com/cron-expression-generator-quartz.html
                  //.WithCronSchedule("5 1/10,5/10,9/10 * * * ?"));
                  .WithCronSchedule("0/15 * * ? * * *"));

            /*
            options.AddJob<GenerateSignalsJob>(GenerateSignalsJobKeys.Key, j => j
                   .WithDescription("Generate trading signals"));

            options.AddTrigger(t => t
                  .WithIdentity("Generate trading signals cron trigger")
                  .ForJob(GenerateSignalsJobKeys.Key)
                  .WithCronSchedule("10 9/10 * * * ?"));

            options.AddJob<ExecuteOrdersJob>(ExecuteOrdersJobKeys.Key, j => j
                   .WithDescription("Execute orders by trading signals"));

            options.AddTrigger(t => t
                  .WithIdentity("Execute orders cron trigger")
                  .ForJob(ExecuteOrdersJobKeys.Key)
                  .WithCronSchedule("30 9/10 * * * ?"));

            options.AddJob<LoadPortfolioSnapshotsJob>(LoadPortfolioSnapshotsJobKeys.Key, j => j
                   .WithDescription("Load portfolio snapshots from Tinvest API"));

            options.AddTrigger(t => t
                  .WithIdentity("Tinvest portfolio snapshots loader cron trigger")
                  .ForJob(LoadPortfolioSnapshotsJobKeys.Key)
                  .WithCronSchedule("50 5/10,9/10 * * * ?"));

            options.AddJob<LoadOperationsJob>(LoadOperationsJobKeys.Key, j => j
                   .WithDescription("Load operations from Tinvest API"));

            options.AddTrigger(t => t
                  .WithIdentity("Tinvest operations loader cron trigger")
                  .ForJob(LoadOperationsJobKeys.Key)
                  .WithCronSchedule("50 9/10 * * * ?"));
            */
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }
}
