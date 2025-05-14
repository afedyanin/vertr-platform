using System.Text.Json;
using Grpc.Core;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.Infrastructure.Kafka;
using Vertr.Infrastructure.Kafka.Abstractions;
using Vertr.TinvestGateway.Contracts;
using Vertr.TinvestGateway.Converters;
using Vertr.TinvestGateway.Settings;

namespace Vertr.TinvestGateway.BackgroundServices;

public class OrderStateStreamService : StreamServiceBase
{
    private readonly OrderStateStreamSettings _streamSettings;
    private readonly IProducerWrapper<string, OrderState> _producerWrapper;
    private readonly string? _topicName;

    protected override bool IsEnabled => _streamSettings.IsEnabled;

    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
    };


    public OrderStateStreamService(
        IOptions<OrderStateStreamSettings> streamSettings,
        IProducerWrapper<string, OrderState> producerWrapper,
        IOptions<KafkaSettings> kafkaSettings,
        IOptions<TinvestSettings> tinvestOptions,
        InvestApiClient investApiClient,
        ILogger<OrderStateStreamService> logger) :
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
        var request = new Tinkoff.InvestApi.V1.OrderStateStreamRequest();
        request.Accounts.Add(TinvestSettings.Accounts);

        using var stream = InvestApiClient.OrdersStream.OrderStateStream(request, headers: null, deadline, stoppingToken);

        await foreach (var response in stream.ResponseStream.ReadAllAsync(stoppingToken))
        {
            if (response.PayloadCase == Tinkoff.InvestApi.V1.OrderStateStreamResponse.PayloadOneofCase.OrderState)
            {
                var json = JsonSerializer.Serialize(response.OrderState, options: _jsonOptions);
                logger.LogWarning($"Order state raw data: {json}");

                var orderState = response.OrderState.Convert();
                var accountId = response.OrderState.AccountId;

                logger.LogDebug($"Order state received: {orderState}");

                if (string.IsNullOrEmpty(_topicName))
                {
                    logger.LogWarning($"Skipping producing to Kafka. Unknown topic name.");
                    continue;
                }

                await _producerWrapper.Produce(_topicName, accountId, orderState, DateTime.UtcNow, stoppingToken);
            }
            else if (response.PayloadCase == Tinkoff.InvestApi.V1.OrderStateStreamResponse.PayloadOneofCase.Ping)
            {
                logger.LogDebug($"Order state ping received: {response.Ping}");
            }
            else if (response.PayloadCase == Tinkoff.InvestApi.V1.OrderStateStreamResponse.PayloadOneofCase.Subscription)
            {
                logger.LogInformation($"Order state subscriptions received: {response.Subscription}");
            }
        }
    }
}
