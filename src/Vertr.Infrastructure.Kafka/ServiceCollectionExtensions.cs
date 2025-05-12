using Microsoft.Extensions.DependencyInjection;
using Vertr.Infrastructure.Kafka.Abstractions;

namespace Vertr.Infrastructure.Kafka;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaSettings(
        this IServiceCollection serviceCollection,
        Action<KafkaSettings> configure)
    {
        serviceCollection.AddOptions<KafkaSettings>().Configure(configure);
        serviceCollection.AddSingleton<IConsumerFactory, ConsumerFactory>();
        serviceCollection.AddSingleton<IProducerFactory, ProducerFactory>();

        return serviceCollection;
    }

    public static IServiceCollection AddKafkaConsumer<TKey, TValue>(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IConsumerWrapper<TKey, TValue>, ConsumerWrapper<TKey, TValue>>();
        return serviceCollection;
    }

    public static IServiceCollection AddKafkaProducer<TKey, TValue>(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IProducerWrapper<TKey, TValue>, ProducerWrapper<TKey, TValue>>();
        return serviceCollection;
    }
}
