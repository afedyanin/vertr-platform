using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.TinvestGateway.Application.Converters;
using Vertr.TinvestGateway.Application.Settings;
using Vertr.TinvestGateway.Contracts.Requests;

namespace Vertr.TinvestGateway.Application.BackgroundServices;

public class OrderTradesStreamService : StreamServiceBase
{
    protected override bool IsEnabled => TinvestSettings.OrderTradeStreamEnabled;

    public OrderTradesStreamService(
        IOptions<TinvestSettings> tinvestOptions,
        InvestApiClient investApiClient,
        IMediator mediator,
        ILogger<OrderTradesStreamService> logger) :
            base(tinvestOptions, investApiClient, mediator, logger)
    {
    }

    protected override async Task Subscribe(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken stoppingToken = default)
    {
        var request = new Tinkoff.InvestApi.V1.TradesStreamRequest();
        request.Accounts.Add(TinvestSettings.Accounts);

        using var stream = InvestApiClient.OrdersStream.TradesStream(request, headers: null, deadline, stoppingToken);

        await foreach (var response in stream.ResponseStream.ReadAllAsync(stoppingToken))
        {
            if (response.PayloadCase == Tinkoff.InvestApi.V1.TradesStreamResponse.PayloadOneofCase.OrderTrades)
            {
                var orderTradesRequest = new HandleOrderTradesRequest
                {
                    OrderTrades = response.OrderTrades.Convert(),
                };
                
                await Mediator.Send(orderTradesRequest, stoppingToken);
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
