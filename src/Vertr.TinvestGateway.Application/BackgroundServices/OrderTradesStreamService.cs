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

public class OrderTradesStreamService : StreamServiceBase
{
    private readonly IPortfolioManager _portfolioManager;

    protected override bool IsEnabled => TinvestSettings.OrderTradeStreamEnabled;

    public OrderTradesStreamService(
        IPortfolioManager portfolioManager,
        IOptions<TinvestSettings> tinvestOptions,
        InvestApiClient investApiClient,
        IMediator mediator,
        ILogger<OrderTradesStreamService> logger) :
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
        var request = new Tinkoff.InvestApi.V1.TradesStreamRequest();
        request.Accounts.Add(accounts);

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
