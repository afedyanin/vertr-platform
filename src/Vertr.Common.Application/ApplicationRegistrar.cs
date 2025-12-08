using Disruptor;
using Disruptor.Dsl;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Vertr.Common.Application.Clients;
using Vertr.Common.Application.EventHandlers;
using Vertr.Common.Application.Services;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application;

public static class ApplicationRegistrar
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<MarketDataPredictor>();
        services.AddSingleton<TradingSignalsGenerator>();
        services.AddSingleton<PortfolioPositionHandler>();
        services.AddSingleton<OrderExecutionHandler>();

        services.AddSingleton<IPortfolioService, PortfolioService>();

        return services;
    }

    public static IServiceCollection AddTinvestGateway(this IServiceCollection services, string baseAddress)
    {
        services
           .AddRefitClient<ITinvestGatewayClient>(
               new RefitSettings
               {
                   ContentSerializer = new SystemTextJsonContentSerializer(JsonOptions.DefaultOptions)
               })
           .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseAddress));

        return services;
    }

    public static Disruptor<CandlestickReceivedEvent> CreateCandlestickPipeline(IServiceProvider serviceProvider)
    {
        var disruptor = new Disruptor<CandlestickReceivedEvent>(
            () => new CandlestickReceivedEvent(),
            ringBufferSize: 1024,
            taskScheduler: TaskScheduler.Default,
            producerType: ProducerType.Single,
            waitStrategy: new BlockingWaitStrategy());

        disruptor.HandleEventsWith(
            serviceProvider.GetRequiredService<MarketDataPredictor>(),
            serviceProvider.GetRequiredService<TradingSignalsGenerator>(),
            serviceProvider.GetRequiredService<PortfolioPositionHandler>(),
            serviceProvider.GetRequiredService<OrderExecutionHandler>()
            );

        return disruptor;
    }
}
