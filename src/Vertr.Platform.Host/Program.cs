using System.Text.Json.Serialization;
using Google.Api;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.Backtest.Application;
using Vertr.Backtest.DataAccess;
using Vertr.Backtest.WebApi;
using Vertr.Backtest.WebApi.Hubs;
using Vertr.Infrastructure.Common.Jobs;
using Vertr.Infrastructure.Common.Mediator;
using Vertr.MarketData.Application;
using Vertr.MarketData.DataAccess;
using Vertr.MarketData.WebApi;
using Vertr.OrderExecution.Application;
using Vertr.OrderExecution.DataAccess;
using Vertr.OrderExecution.WebApi;
using Vertr.OrderExecution.WebApi.Hubs;
using Vertr.Platform.BlazorUI.Components;
using Vertr.Platform.Host.BackgroundServices;
using Vertr.Platform.Host.Hubs;
using Vertr.Platform.Host.StockTicker;
using Vertr.PortfolioManager.Application;
using Vertr.PortfolioManager.DataAccess;
using Vertr.PortfolioManager.WebApi;
using Vertr.PortfolioManager.WebApi.Hubs;
using Vertr.Strategies.Application;
using Vertr.Strategies.DataAccess;
using Vertr.Strategies.WebApi;
using Vertr.TinvestGateway;
using Vertr.TinvestGateway.WebApi;

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
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
            .AddApplicationPart(typeof(BacktestController).Assembly)
            .AddApplicationPart(typeof(TinvestGatewayController).Assembly)
            .AddApplicationPart(typeof(CandlesController).Assembly)
            .AddApplicationPart(typeof(OrderEventsController).Assembly)
            .AddApplicationPart(typeof(PortfoliosController).Assembly)
            .AddApplicationPart(typeof(StrategiesController).Assembly);

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

        builder.Services.AddPortfolioManager();
        builder.Services.AddPortfolioManagerDataAccess(connectionString!);

        // Hangfire
        var hfConnectionString = configuration.GetConnectionString(_hangfireConnStringName);
        builder.Services.AddHangfire(hfConnectionString!);

        // Mediator
        builder.Services.AddMediator();

        builder.Services.AddSignalR();

        // Hubs
        //builder.Services.AddHostedService<StockPricesUpdatingService>();
        builder.Services.AddSingleton<StockTickerSubject>();
        builder.Services.AddSingleton<IStockTickerObservable>(x => x.GetRequiredService<StockTickerSubject>());
        builder.Services.AddSingleton<IStockTickerDataHandler>(x => x.GetRequiredService<StockTickerSubject>());

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

        app.MapHub<StocksHub>("/stocksHub");
        app.MapHub<BacktestProgressHub>("/backtestsHub");
        app.MapHub<OrderEventsHub>("/orderEventsHub");
        app.MapHub<PositionsHub>("/positionsHub");

        app.UseHangfireDashboard();

        app.Run();
    }
}
