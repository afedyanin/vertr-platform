using Grpc.Core;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;

namespace Vertr.TinvestGateway.BackgroundServices;

public class PositionsStreamService : StreamServiceBase
{
    public PositionsStreamService(
        IOptions<TinvestSettings> options,
        InvestApiClient investApiClient,
        ILogger<PositionsStreamService> logger) : base(options, investApiClient, logger)
    {
    }

    protected override async Task Subscribe(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken stoppingToken = default)
    {
        var request = new Tinkoff.InvestApi.V1.PositionsStreamRequest();
        request.Accounts.Add(Settings.Accounts);

        using (var stream = InvestApiClient.OperationsStream.PositionsStream(request, headers: null, deadline, stoppingToken))
        {
            await foreach (var response in stream.ResponseStream.ReadAllAsync(stoppingToken))
            {
                if (response.PayloadCase == Tinkoff.InvestApi.V1.PositionsStreamResponse.PayloadOneofCase.Position)
                {
                    var position = response.Position;
                    logger.LogWarning($"Position received: {position}");
                }
                else if (response.PayloadCase == Tinkoff.InvestApi.V1.PositionsStreamResponse.PayloadOneofCase.Ping)
                {
                    logger.LogDebug($"Position ping received: {response.Ping}");
                }
                else if (response.PayloadCase == Tinkoff.InvestApi.V1.PositionsStreamResponse.PayloadOneofCase.Subscriptions)
                {
                    var subs = response.Subscriptions;
                    var all = subs.Accounts.ToArray()
                        .Select(s => $"AccountId={s.AccountId} Status={s.SubscriptionStatus}").ToArray();

                    logger.LogInformation($"Positions subscriptions received: {string.Join(',', all)}");
                }
            }
        }
    }
}
