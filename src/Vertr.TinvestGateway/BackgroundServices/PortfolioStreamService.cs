using Grpc.Core;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.Infrastructure.Kafka;
using Vertr.Infrastructure.Kafka.Abstractions;
using Vertr.TinvestGateway.Contracts;
using Vertr.TinvestGateway.Converters;
using Vertr.TinvestGateway.Settings;

namespace Vertr.TinvestGateway.BackgroundServices;

public class PortfolioStreamService : StreamServiceBase
{
    private readonly PortfolioStreamSettings _streamSettings;
    private readonly IProducerWrapper<string, PortfolioResponse> _producerWrapper;
    private readonly string? _topicName;

    protected override bool IsEnabled => _streamSettings.IsEnabled;

    public PortfolioStreamService(
        IOptions<PortfolioStreamSettings> streamSettings,
        IProducerWrapper<string, PortfolioResponse> producerWrapper,
        IOptions<KafkaSettings> kafkaSettings,
        IOptions<TinvestSettings> tinvestOptions,
        InvestApiClient investApiClient,
        ILogger<PortfolioStreamService> logger) :
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
        var request = new Tinkoff.InvestApi.V1.PortfolioStreamRequest();
        request.Accounts.Add(TinvestSettings.Accounts);

        using var stream = InvestApiClient.OperationsStream.PortfolioStream(request, headers: null, deadline, stoppingToken);

        await foreach (var response in stream.ResponseStream.ReadAllAsync(stoppingToken))
        {
            if (response.PayloadCase == Tinkoff.InvestApi.V1.PortfolioStreamResponse.PayloadOneofCase.Portfolio)
            {
                var portfolio = response.Portfolio.Convert();

                logger.LogDebug($"Portfolio received: {portfolio}");

                if (string.IsNullOrEmpty(_topicName))
                {
                    logger.LogWarning($"Skipping producing to Kafka. Unknown topic name.");
                    continue;
                }

                await _producerWrapper.Produce(_topicName, portfolio.AccountId, portfolio, DateTime.UtcNow, stoppingToken);
            }
            else if (response.PayloadCase == Tinkoff.InvestApi.V1.PortfolioStreamResponse.PayloadOneofCase.Ping)
            {
                logger.LogDebug($"Portfolio ping received: {response.Ping}");
            }
            else if (response.PayloadCase == Tinkoff.InvestApi.V1.PortfolioStreamResponse.PayloadOneofCase.Subscriptions)
            {
                var subs = response.Subscriptions;
                var all = subs.Accounts.ToArray()
                    .Select(s => $"AccountId={s.AccountId} Status={s.SubscriptionStatus}").ToArray();

                logger.LogInformation($"Portfolio subscriptions received: {string.Join(',', all)}");
            }
        }
    }
}
