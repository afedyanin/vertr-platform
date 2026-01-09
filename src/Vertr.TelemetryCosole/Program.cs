using System.Diagnostics.Metrics;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
namespace Vertr.TelemetryCosole;

internal sealed class Program
{
    static async Task Main(string[] args)
    {
        var myMeter = new Meter("MyCompany.MyConsoleApp", "1.0.0");
        var myCounter = myMeter.CreateCounter<long>("orders_processed_total", description: "Total number of processed orders");

        using var meterProvider = Sdk.CreateMeterProviderBuilder()
            .AddMeter("MyCompany.MyConsoleApp") // Подписываемся на наш Meter
            .AddOtlpExporter((options, metricReadeOptions) =>
            {
                options.Endpoint = new Uri("http://localhost:9090/api/v1/otlp/v1/metrics");
                options.Protocol = OtlpExportProtocol.HttpProtobuf;

                metricReadeOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 1000;
            })
            .Build();

        Console.WriteLine("Отправка метрик запущена. Нажмите любую клавишу для выхода...");

        while (!Console.KeyAvailable)
        {
            myCounter.Add(1, new KeyValuePair<string, object?>("env", "production"));
            Console.WriteLine("Метрика 'orders_processed_total' увеличена.");
            await Task.Delay(5000); // Интервал генерации данных
        }
    }
}
