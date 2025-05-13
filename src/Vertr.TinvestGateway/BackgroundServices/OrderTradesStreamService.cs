using Grpc.Core;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;

namespace Vertr.TinvestGateway.BackgroundServices;

public class OrderTradesStreamService : StreamServiceBase
{
    public OrderTradesStreamService(
        IOptions<TinvestSettings> options,
        InvestApiClient investApiClient,
        ILogger<OrderTradesStreamService> logger) : base(options, investApiClient, logger)
    {
    }

    protected override async Task Subscribe(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken stoppingToken = default)
    {
        var request = new Tinkoff.InvestApi.V1.TradesStreamRequest();
        request.Accounts.Add(Settings.Accounts);

        using var stream = InvestApiClient.OrdersStream.TradesStream(request, headers: null, deadline, stoppingToken);

        await foreach (var response in stream.ResponseStream.ReadAllAsync(stoppingToken))
        {
            if (response.PayloadCase == Tinkoff.InvestApi.V1.TradesStreamResponse.PayloadOneofCase.OrderTrades)
            {
                foreach (var trade in response.OrderTrades.Trades)
                {
                    logger.LogWarning($"Trade received: {trade}");
                }
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
