using System.Text.Json.Serialization;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.Infrastructure.Hangfire;
using Vertr.MarketData.Application;
using Vertr.MarketData.DataAccess;
using Vertr.Platform.Host.Components;
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
        builder.Services.AddHangfire(builder.Configuration.GetConnectionString("HangfireConnection")!);


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

        app.UseHangfireDashboard();

        app.Run();
    }
}
