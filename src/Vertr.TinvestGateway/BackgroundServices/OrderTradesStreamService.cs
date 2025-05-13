using Grpc.Core;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.Infrastructure.Kafka;
using Vertr.Infrastructure.Kafka.Abstractions;
using Vertr.TinvestGateway.Contracts;
using Vertr.TinvestGateway.Converters;
using Vertr.TinvestGateway.Settings;

namespace Vertr.TinvestGateway.BackgroundServices;

public class OrderTradesStreamService : StreamServiceBase
{
    private readonly OrderTradesStreamSettings _streamSettings;
    private readonly IProducerWrapper<string, OrderTrades> _producerWrapper;
    private readonly string? _topicName;

    public OrderTradesStreamService(
        IOptions<OrderTradesStreamSettings> streamSettings,
        IProducerWrapper<string, OrderTrades> producerWrapper,
        IOptions<KafkaSettings> kafkaSettings,
        IOptions<TinvestSettings> tinvestOptions,
        InvestApiClient investApiClient,
        ILogger<OrderTradesStreamService> logger) :
            base(kafkaSettings, tinvestOptions, investApiClient, logger)
    {
        _streamSettings = streamSettings.Value;
        _producerWrapper = producerWrapper;
        _topicName = KafkaSettings.GetTopicByKey(_streamSettings.TopicKey);
    }

    protected override async Task Subscribe(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken stoppingToken = default)
    {
        if (!_streamSettings.IsEnabled)
        {
            logger.LogWarning($"{nameof(OrderTradesStreamService)} is disabled.");
            return;
        }

        var request = new Tinkoff.InvestApi.V1.TradesStreamRequest();
        request.Accounts.Add(TinvestSettings.Accounts);

        using var stream = InvestApiClient.OrdersStream.TradesStream(request, headers: null, deadline, stoppingToken);

        await foreach (var response in stream.ResponseStream.ReadAllAsync(stoppingToken))
        {
            if (response.PayloadCase == Tinkoff.InvestApi.V1.TradesStreamResponse.PayloadOneofCase.OrderTrades)
            {
                var orderTrades = response.OrderTrades.Convert();

                logger.LogDebug($"Order trades received: {orderTrades}");

                if (string.IsNullOrEmpty(_topicName))
                {
                    logger.LogWarning($"Skipping producing to Kafka. Unknown topic name.");
                    continue;
                }

                await _producerWrapper.Produce(_topicName, orderTrades.AccountId, orderTrades, DateTime.UtcNow, stoppingToken);
            }
            else if (response.PayloadCase == Tinkoff.InvestApi.V1.TradesStreamResponse.PayloadOneofCase.Ping)
            {
                logger.LogDebug($"Trades ping received: {response.Ping}");
            }
            else if (response.PayloadCase == Tinkoff.InvestApi.V1.TradesStreamResponse.PayloadOneofCase.Subscription)
            {
                logger.LogInformation($"Trades subscription received: {response.Subscription}");
            }
        }
    }
}
