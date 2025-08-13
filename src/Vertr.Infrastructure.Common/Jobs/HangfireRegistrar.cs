using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Infrastructure.Common.Jobs.Filters;
using Vertr.Platform.Common.Jobs;

namespace Vertr.Infrastructure.Common.Jobs;

public static class HangfireRegistrar
{
    public static IServiceCollection AddHangfire(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IJobScheduler, HangfireJobScheduler>();

        // Hangfire
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(
                c => c.UseNpgsqlConnection(connectionString)
                    , new PostgreSqlStorageOptions
                    {
                        PrepareSchemaIfNecessary = true,
                        QueuePollInterval = TimeSpan.FromSeconds(5),
                        InvisibilityTimeout = TimeSpan.FromMinutes(1),
                        JobExpirationCheckInterval = TimeSpan.FromHours(2),
                    }
        ));

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = 3;
            options.SchedulePollingInterval = TimeSpan.FromSeconds(5);
            options.HeartbeatInterval = TimeSpan.FromSeconds(10);
            options.StopTimeout = TimeSpan.FromSeconds(15);
            options.ServerTimeout = TimeSpan.FromMinutes(15);
            options.ShutdownTimeout = TimeSpan.FromSeconds(30);
            options.CancellationCheckInterval = TimeSpan.FromSeconds(3);
        });

        GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });

        return services;
    }

    public static void UseHangfireDashboard(this IApplicationBuilder application)
    {
        application.UseHangfireDashboard(options: new DashboardOptions
        {
            Authorization = [new SkipAuthorizationFilter()]
        });
    }
}
