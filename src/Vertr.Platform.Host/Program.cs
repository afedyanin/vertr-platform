
using System.Text.Json.Serialization;
using Vertr.MarketData.Application;
using Vertr.OrderExecution.Application;
using Vertr.OrderExecution.DataAccess;
using Vertr.PortfolioManager.Application;
using Vertr.PortfolioManager.DataAccess;
using Vertr.TinvestGateway.Application;

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

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var configuration = builder.Configuration;
        var connectionString = configuration.GetConnectionString(_connStringName);

        // Add modules
        builder.Services.AddTinvestGateway(configuration);
        builder.Services.AddTinvestOrders();

        builder.Services.AddMarketData();
        builder.Services.AddOrderExecution();
        builder.Services.AddOrderExecutionDataAccess(connectionString!);
        builder.Services.AddPortfolioManager();
        builder.Services.AddPortfolioManagerDataAccess(connectionString!);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
