using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.TinvestGateway.Application.Converters;
using Vertr.TinvestGateway.Application.Settings;
using Vertr.TinvestGateway.Contracts.Requests;

namespace Vertr.TinvestGateway.Application.BackgroundServices;

public class PositionStreamService : StreamServiceBase
{
    protected override bool IsEnabled => TinvestSettings.PositionStreamEnabled;


    public PositionStreamService(
        IOptions<TinvestSettings> tinvestOptions,
        InvestApiClient investApiClient,
        IMediator mediator,
        ILogger<PositionStreamService> logger) :
            base(tinvestOptions, investApiClient, mediator, logger)
    {
    }

    protected override async Task Subscribe(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken stoppingToken = default)
    {
        var request = new Tinkoff.InvestApi.V1.PositionsStreamRequest();
        
        // TODO: Refactor this
        request.Accounts.Add(TinvestSettings.Accounts);

        using var stream = InvestApiClient.OperationsStream.PositionsStream(request, headers: null, deadline, stoppingToken);

        await foreach (var response in stream.ResponseStream.ReadAllAsync(stoppingToken))
        {
            if (response.PayloadCase == Tinkoff.InvestApi.V1.PositionsStreamResponse.PayloadOneofCase.Position)
            {
                var positionRequest = new HandlePositionRequest
                {
                    Positions = response.Position.Convert()
                };

                await Mediator.Send(positionRequest, stoppingToken);
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
