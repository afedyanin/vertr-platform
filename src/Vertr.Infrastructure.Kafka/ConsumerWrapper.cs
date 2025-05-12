using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Vertr.Infrastructure.Kafka.Abstractions;

namespace Vertr.Infrastructure.Kafka;
internal sealed class ConsumerWrapper<TKey, TValue> : IConsumerWrapper<TKey, TValue>
{
    private static readonly TimeSpan _consumerRestartDelay = TimeSpan.FromMilliseconds(100);

    private readonly IConsumerFactory _consumerFactory;
    private readonly ILogger<ConsumerWrapper<TKey, TValue>> _logger;

    public ConsumerWrapper(
        IConsumerFactory consumerFactory,
        ILogger<ConsumerWrapper<TKey, TValue>> logger)
    {
        _consumerFactory = consumerFactory;
        _logger = logger;
    }

    public async Task Consume(
        string[] topics,
        Func<ConsumeResult<TKey, TValue>, CancellationToken, Task> handler,
        bool readFromBegining = false,
        CancellationToken stoppingToken = default
        )
    {
        _logger.LogInformation($"Start consuming. GroupId={_consumerFactory.ConsumerConfig.GroupId}");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await StartAsync(topics, handler, readFromBegining, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogDebug("Cancell consuming...");
                break;
            }
            catch (Exception ex)
            {
                if (ex.IsCritical())
                {
                    _logger.LogCritical(ex, $"Critical exception. Message={ex}.");
                    throw;
                }

                _logger.LogError(ex, $"Consuming exception. Message={ex}. Restarting consumer.");
            }

            await Task.Delay(_consumerRestartDelay);
        }

        _logger.LogDebug("End consuming.");
    }

    private async Task StartAsync(
        string[] topics,
        Func<ConsumeResult<TKey, TValue>, CancellationToken, Task> handler,
        bool readFromBegining,
        CancellationToken stoppingToken)
    {
        _logger.LogDebug($"Creating consumer.");

        var consumer = _consumerFactory.CreateConsumer<TKey, TValue>(readFromBegining);

        try
        {
            _logger.LogDebug($"Subscribing on topics: {string.Join(", ", topics)}");
            consumer.Subscribe(topics);
            await StartConsumerLoop(consumer, handler, stoppingToken);
        }
        finally
        {
            // Calls Dispose inside.
            consumer.Close();
        }

        _logger.LogDebug($"Consumer closed.");
    }

    private async Task StartConsumerLoop(
        IConsumer<TKey, TValue> consumer,
        Func<ConsumeResult<TKey, TValue>, CancellationToken, Task> handler,
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            ConsumeResult<TKey, TValue>? result = null;

            try
            {
                _logger.LogDebug($"Consuming result...");
                result = consumer.Consume(stoppingToken);

                if (result == null)
                {
                    _logger.LogDebug($"Empty consuming result. Skipping...");
                    continue;
                }

                _logger.LogDebug($"Start handling consumer result.");
                await HandleResultSafe(result, handler, stoppingToken);
                _logger.LogDebug($"End handling consumer result.");

                if (!IsAutoCommitEnabled)
                {
                    _logger.LogDebug($"Commiting consumer result.");
                    consumer.Commit(result);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Consumer loop cancelled.");
                break;
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, $"Consume error: Reason={ex.Error.Reason} Message={ex.Message} IsFatal={ex.Error.IsFatal}");

                // https://github.com/edenhill/librdkafka/blob/master/INTRODUCTION.md#fatal-consumer-errors
                if (ex.Error.IsFatal)
                {
                    throw;
                }

                continue;
            }
        }
    }

    private bool IsAutoCommitEnabled
        => !_consumerFactory.ConsumerConfig.EnableAutoCommit.HasValue // true by default
        || _consumerFactory.ConsumerConfig.EnableAutoCommit.Value;

    private async Task HandleResultSafe(
        ConsumeResult<TKey, TValue> result,
        Func<ConsumeResult<TKey, TValue>, CancellationToken, Task> handler,
        CancellationToken cancellationToken)
    {
        try
        {
            await handler(result, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation($"Result handler cancelled.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, $"Result handler error. Message={ex.Message}");
        }
    }
}
