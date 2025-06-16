using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.PortfolioManager.Contracts.Interfaces;
using Vertr.TinvestGateway.Application.Converters;
using Vertr.TinvestGateway.Application.Settings;
using Vertr.TinvestGateway.Contracts.Requests;

namespace Vertr.TinvestGateway.Application.BackgroundServices;

public class PortfolioStreamService : StreamServiceBase
{
    private readonly IPortfolioManager _portfolioManager;
    protected override bool IsEnabled => TinvestSettings.PositionStreamEnabled;

    public PortfolioStreamService(
        IPortfolioManager portfolioManager,
        IOptions<TinvestSettings> tinvestOptions,
        InvestApiClient investApiClient,
        IMediator mediator,
        ILogger<PortfolioStreamService> logger) :
            base(tinvestOptions, investApiClient, mediator, logger)
    {
        _portfolioManager = portfolioManager;
    }

    protected override async Task Subscribe(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken stoppingToken = default)
    {
        var accounts = await _portfolioManager.GetActiveAccounts();
        var request = new Tinkoff.InvestApi.V1.PortfolioStreamRequest();

        request.Accounts.Add(accounts);

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
