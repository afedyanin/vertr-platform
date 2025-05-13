using Grpc.Core;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;

namespace Vertr.TinvestGateway.BackgroundServices;

public class OrderStateStreamService : StreamServiceBase
{
    public OrderStateStreamService(
        IOptions<TinvestSettings> options,
        InvestApiClient investApiClient,
        ILogger<OrderStateStreamService> logger) : base(options, investApiClient, logger)
    {
    }

    protected override async Task Subscribe(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken stoppingToken = default)
    {
        var request = new Tinkoff.InvestApi.V1.OrderStateStreamRequest();
        request.Accounts.Add(Settings.Accounts);

        using var stream = InvestApiClient.OrdersStream.OrderStateStream(request, headers: null, deadline, stoppingToken);

        await foreach (var response in stream.ResponseStream.ReadAllAsync(stoppingToken))
        {
            if (response.PayloadCase == Tinkoff.InvestApi.V1.OrderStateStreamResponse.PayloadOneofCase.OrderState)
            {
                foreach (var trade in response.OrderState.Trades)
                {
                    logger.LogWarning($"Order state Trade received: {trade}");
                }
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
