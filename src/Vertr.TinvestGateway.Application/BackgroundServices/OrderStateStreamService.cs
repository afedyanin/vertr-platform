using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.TinvestGateway.Application.Converters;
using Vertr.TinvestGateway.Application.Settings;
using Vertr.TinvestGateway.Contracts.Requests;

namespace Vertr.TinvestGateway.Application.BackgroundServices;

public class OrderStateStreamService : StreamServiceBase
{
    protected override bool IsEnabled => TinvestSettings.OrderStateStreamEnabled;

    public OrderStateStreamService(
        IOptions<TinvestSettings> tinvestOptions,
        InvestApiClient investApiClient,
        IMediator mediator,
        ILogger<OrderStateStreamService> logger) :
            base(tinvestOptions, investApiClient, mediator, logger)
    {
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
                var orderStateRequest = new HandleOrderStateRequest
                {
                    OrderState = response.OrderState.Convert(),
                    AccountId = response.OrderState.AccountId,
                };

                await Mediator.Send(orderStateRequest, stoppingToken);
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
