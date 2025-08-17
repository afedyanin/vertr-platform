using System.Text.Json.Serialization;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.Backtest.Application;
using Vertr.Backtest.DataAccess;
using Vertr.Infrastructure.Common.Jobs;
using Vertr.Infrastructure.Common.Mediator;
using Vertr.MarketData.Application;
using Vertr.MarketData.DataAccess;
using Vertr.Platform.Host.Components;
using Vertr.Strategies.Application;
using Vertr.Strategies.DataAccess;
using Vertr.TinvestGateway;
using Vertr.OrderExecution.Application;
using Vertr.OrderExecution.DataAccess;


namespace Vertr.Platform.Host;

public class Program
{
    private const string _connStringName = "VertrDbConnection";
    private const string _hangfireConnStringName = "HangfireConnection";
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

        builder.Services.AddBacktests();
        builder.Services.AddBacktestDataAccess(connectionString!);

        builder.Services.AddOrderExecution();
        builder.Services.AddOrderExecutionDataAccess(connectionString!);

        // Hangfire
        var hfConnectionString = configuration.GetConnectionString(_hangfireConnStringName);
        builder.Services.AddHangfire(hfConnectionString!);

        // Mediator
        builder.Services.AddMediator();

        /*
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

        app.UseHangfireDashboard();

        app.Run();
    }
}
