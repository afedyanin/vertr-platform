using System.Text.Json.Serialization;
using Tinkoff.InvestApi;
using Vertr.Infrastructure.Kafka;
using Vertr.TinvestGateway.BackgroundServices;
using Vertr.TinvestGateway.Contracts;
using Vertr.TinvestGateway.Settings;

namespace Vertr.TinvestGateway;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<RpcExceptionHandler>();

        builder.Services.AddOptions<TinvestSettings>().BindConfiguration(nameof(TinvestSettings));
        builder.Services.AddOptions<KafkaSettings>().BindConfiguration(nameof(KafkaSettings));
        builder.Services.AddOptions<MarketDataStreamSettings>().BindConfiguration(nameof(MarketDataStreamSettings));
        builder.Services.AddOptions<OrderStateStreamSettings>().BindConfiguration(nameof(OrderStateStreamSettings));
        builder.Services.AddOptions<OrderTradesStreamSettings>().BindConfiguration(nameof(OrderTradesStreamSettings));
        builder.Services.AddOptions<PortfolioStreamSettings>().BindConfiguration(nameof(PortfolioStreamSettings));
        builder.Services.AddOptions<PositionsStreamSettings>().BindConfiguration(nameof(PositionsStreamSettings));

        builder.Services.AddKafkaSettings(settings => builder.Configuration.Bind(nameof(KafkaSettings), settings));

        builder.Services.AddKafkaProducer<string, Candle>();
        builder.Services.AddKafkaProducer<string, OrderState>();
        builder.Services.AddKafkaProducer<string, OrderTrades>();
        builder.Services.AddKafkaProducer<string, PortfolioResponse>();
        builder.Services.AddKafkaProducer<string, PositionsResponse>();

        builder.Services.AddInvestApiClient((_, settings) => builder.Configuration.Bind($"{nameof(TinvestSettings)}:{nameof(InvestApiSettings)}", settings));

        builder.Services.AddControllers().AddJsonOptions(options
            => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        builder.Services.AddHostedService<OrderTradesStreamService>();
        builder.Services.AddHostedService<OrderStateStreamService>();
        builder.Services.AddHostedService<MarketDataStreamService>();
        builder.Services.AddHostedService<PortfolioStreamService>();
        builder.Services.AddHostedService<PositionsStreamService>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.UseExceptionHandler();

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
