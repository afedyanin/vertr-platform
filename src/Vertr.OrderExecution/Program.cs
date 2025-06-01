using System.Text.Json;
using Vertr.Infrastructure.Kafka;
using Vertr.OrderExecution.Application;
using Vertr.OrderExecution.Application.Abstractions;
using Vertr.OrderExecution.BackgroundServices;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.DataAccess;
using Vertr.TinvestGateway.Contracts;

namespace Vertr.OrderExecution;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configuration = builder.Configuration;

        var appSettings = new OrderExecutionSettings();
        configuration.GetSection(nameof(OrderExecutionSettings)).Bind(appSettings);
        builder.Services.AddOptions<OrderExecutionSettings>().BindConfiguration(nameof(OrderExecutionSettings));

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

        builder.Services.AddKafkaConsumer<string, OrderState>();
        builder.Services.AddKafkaConsumer<string, OrderTrades>();
        builder.Services.AddKafkaProducer<string, OrderOperation>();

        builder.Services.AddHostedService<OrderStateConsumerService>();
        builder.Services.AddHostedService<OrderTradesConsumerService>();
        builder.Services.AddSingleton<IOperationsPublisher, KafkaOperationsPublisher>();

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
