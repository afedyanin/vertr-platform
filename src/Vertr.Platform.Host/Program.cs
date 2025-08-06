using System.Text.Json.Serialization;
using Microsoft.FluentUI.AspNetCore.Components;
using Quartz;
using Vertr.MarketData.Application;
using Vertr.MarketData.DataAccess;
using Vertr.Platform.Host.Components;
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

        // Add quatrz jobs

        // base configuration from appsettings.json
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

        app.Run();
    }
}
