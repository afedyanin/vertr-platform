using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;

namespace Vertr.Experimental.MetricsApp;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton<ProductMetrics>();

        using var meterProvider = Sdk.CreateMeterProviderBuilder()
            .AddMeter("MyApp.Products") // Подписываемся на наш Meter
            .AddOtlpExporter((options, metricReadeOptions) =>
            {
                options.Endpoint = new Uri("http://localhost:9090/api/v1/otlp/v1/metrics");
                options.Protocol = OtlpExportProtocol.HttpProtobuf;

                metricReadeOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 1000;
            })
            .Build();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
