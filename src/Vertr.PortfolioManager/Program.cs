
using System.Text.Json;
using Vertr.Infrastructure.Kafka;
using Vertr.PortfolioManager.Application;
using Vertr.PortfolioManager.BackgroundServices;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.DataAccess;

namespace Vertr.PortfolioManager;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        var appSettings = new PortfolioManagerSettings();
        configuration.GetSection(nameof(PortfolioManagerSettings)).Bind(appSettings);
        builder.Services.AddOptions<PortfolioManagerSettings>().BindConfiguration(nameof(PortfolioManagerSettings));

        builder.Services.AddTinvestGateway(c => c.BaseAddress = new Uri(appSettings.TinvestGatewayUrl));

        builder.Services.AddKafkaSettings(
            settings =>
            {
                builder.Configuration.Bind(nameof(KafkaSettings), settings);
                settings.JsonSerializerOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };
            });

        builder.Services.AddKafkaConsumer<string, PortfolioResponse>();

        builder.Services.AddHostedService<PortfolioConsumerService>();

        builder.Services.AddDataAccess(configuration);
        builder.Services.AddApplication();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
