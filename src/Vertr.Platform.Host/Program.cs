using System.Text.Json.Serialization;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.MarketData.Application;
using Vertr.MarketData.DataAccess;
using Vertr.Platform.Host.Components;
using Vertr.Platform.Host.Filters;
using Vertr.Strategies.Application;
using Vertr.Strategies.DataAccess;
using Vertr.TinvestGateway;

namespace Vertr.Platform.Host;

public class Program
{
    private const string _connStringName = "VertrDbConnection";
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddControllers()
            .AddJsonOptions(options => 
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddFluentUIComponents();

        builder.Services.AddHttpClient("backend", client => client.BaseAddress = new Uri("https://localhost:7085"));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var configuration = builder.Configuration;
        var connectionString = configuration.GetConnectionString(_connStringName);

        // Add modules
        builder.Services.AddTinvestGateways(configuration);
        builder.Services.AddTinvestStreams();

        builder.Services.AddMarketData();
        builder.Services.AddMarketDataAccess(connectionString!);

        builder.Services.AddStrategies();
        builder.Services.AddStrategiesDataAccess(connectionString!);

        // Hangfire
        builder.Services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(
                c => c.UseNpgsqlConnection(builder.Configuration.GetConnectionString("HangfireConnection"))
                    , new PostgreSqlStorageOptions
                    {
                        PrepareSchemaIfNecessary = true,
                        QueuePollInterval = TimeSpan.FromSeconds(5),
                        InvisibilityTimeout = TimeSpan.FromHours(24),
                        JobExpirationCheckInterval = TimeSpan.FromHours(24),
                    }
        ));

        builder.Services.AddHangfireServer(options =>
        {
            options.WorkerCount = 3;
            options.SchedulePollingInterval = TimeSpan.FromSeconds(5);
            options.HeartbeatInterval = TimeSpan.FromSeconds(10);
            options.StopTimeout = TimeSpan.FromSeconds(15);
            options.ServerTimeout = TimeSpan.FromMinutes(15);
            options.ShutdownTimeout = TimeSpan.FromSeconds(30);
        });


        // Add quatrz jobs

        // base configuration from appsettings.json
        /*
        builder.Services.Configure<QuartzOptions>(configuration.GetSection("Quartz"));

        builder.Services.Configure<QuartzOptions>(options =>
        {
            options.Scheduling.IgnoreDuplicates = false; // default: false
            options.Scheduling.OverWriteExistingData = true; // default: true
        });

        builder.Services.AddQuartz(options =>
        {
            options.UseSimpleTypeLoader();
            options.UseInMemoryStore();
            options.UseDefaultThreadPool(tp =>
            {
                tp.MaxConcurrency = 10;
            });

            builder.Services.AddmarketDataQuartzJobs(options);
        });

        builder.Services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });
        */




        //builder.Services.AddTinvestOrders();

        /*
        builder.Services.AddOrderExecution();
        builder.Services.AddOrderExecutionDataAccess(connectionString!);
        builder.Services.AddPortfolioManager();
        builder.Services.AddPortfolioManagerDataAccess(connectionString!);
        */
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.UseAuthorization();

        app.MapControllers();

        app.UseHangfireDashboard(options: new DashboardOptions
        {
            Authorization = [new SkipAuthorizationFilter()]
        });

        app.Run();
    }
}
