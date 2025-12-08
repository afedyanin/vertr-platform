using Microsoft.Extensions.DependencyInjection;
using Disruptor;
using Disruptor.Dsl;
using Vertr.Common.Application.EventHandlers;

namespace Vertr.Common.Application;

public static class ApplicationRegistrar
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<MarketDataPredictor>();
        services.AddSingleton<TradingSignalsGenerator>();
        services.AddSingleton<PortfolioPositionHandler>();
        services.AddSingleton<OrderExecutionHandler>();

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
