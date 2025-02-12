using Quartz;

namespace Vertr.Server;

internal static class QuartzRegistrar
{
    // https://github.dev/246850/Calamus.TaskScheduler/blob/00b1c30c21bf23eaa5cd8497359f39f930562d7d/Calamus.TaskScheduler/Startup.cs#L34#L154
    // https://github.dev/Nfactor26/pixel-identity/blob/fc898f6b2baaa70c0b117d20da93c09054f346f9/src/Pixel.Identity.Provider/Startup.cs#L298#L309

    public static IServiceCollection ConfigureQuatrz(this IServiceCollection services)
    {
        services.AddQuartz(options =>
        {
            options.SchedulerId = "Vertr-Scheduler";
            options.SchedulerName = "Quartz ASP.NET Core Vertr Scheduler";
            options.UseSimpleTypeLoader();
            options.UseInMemoryStore();
            options.UseDefaultThreadPool(tp =>
            {
                tp.MaxConcurrency = 10;
            });
        });

        // Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }
}
