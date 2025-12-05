using System.Text.Json;
using System.Text.Json.Serialization;
using Vertr.TinvestGateway.DataAccess.Redis;
using Vertr.TinvestGateway.Host.BackgroundServices;

namespace Vertr.TinvestGateway.Host;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configuration = builder.Configuration;

        builder.Services.AddControllers()
          .AddJsonOptions(options =>
          {
              options.JsonSerializerOptions.WriteIndented = true;
              options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
              options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
          });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add modules
        builder.Services.AddTinvestGateways(configuration);
        builder.Services.AddRedisDataAccess(configuration);

        builder.Services.AddHostedService<MarketDataStreamService>();
        builder.Services.AddHostedService<OrderTradesStreamService>();
        builder.Services.AddHostedService<OrderStateStreamService>();

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}