using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.TinvestGateway.Application.Converters;
using Vertr.TinvestGateway.Application.Settings;
using Vertr.TinvestGateway.Contracts.Requests;

namespace Vertr.TinvestGateway.Application.BackgroundServices;

public class PortfolioStreamService : StreamServiceBase
{
    protected override bool IsEnabled => TinvestSettings.PositionStreamEnabled;

    public PortfolioStreamService(
        IOptions<TinvestSettings> tinvestOptions,
        InvestApiClient investApiClient,
        IMediator mediator,
        ILogger<PortfolioStreamService> logger) :
            base(tinvestOptions, investApiClient, mediator, logger)
    {
    }

    protected override async Task Subscribe(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken stoppingToken = default)
    {
        var request = new Tinkoff.InvestApi.V1.PortfolioStreamRequest();

        // TODO: Refactor this
        request.Accounts.Add(TinvestSettings.Accounts);

        using var stream = InvestApiClient.OperationsStream.PortfolioStream(request, headers: null, deadline, stoppingToken);

        await foreach (var response in stream.ResponseStream.ReadAllAsync(stoppingToken))
        {
            if (response.PayloadCase == Tinkoff.InvestApi.V1.PortfolioStreamResponse.PayloadOneofCase.Portfolio)
            {
                var portfolioRequest = new HandlePortrolioRequest
                {
                    Portfolio = response.Portfolio.Convert()
                };

                await Mediator.Send(portfolioRequest, stoppingToken);
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
