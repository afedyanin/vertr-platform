using Grpc.Core;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;

namespace Vertr.TinvestGateway.BackgroundServices;

public class PortfolioStreamService : StreamServiceBase
{
    public PortfolioStreamService(
        IOptions<TinvestSettings> options,
        InvestApiClient investApiClient,
        ILogger<PortfolioStreamService> logger) : base(options, investApiClient, logger)
    {
    }

    protected override async Task Subscribe(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken stoppingToken = default)
    {
        var request = new Tinkoff.InvestApi.V1.PortfolioStreamRequest();
        request.Accounts.Add(Settings.Accounts);

        using var stream = InvestApiClient.OperationsStream.PortfolioStream(request, headers: null, deadline, stoppingToken);

        await foreach (var response in stream.ResponseStream.ReadAllAsync(stoppingToken))
        {
            if (response.PayloadCase == Tinkoff.InvestApi.V1.PortfolioStreamResponse.PayloadOneofCase.Portfolio)
            {
                var portfolio = response.Portfolio;
                logger.LogWarning($"Portfolio received: {portfolio}");
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
