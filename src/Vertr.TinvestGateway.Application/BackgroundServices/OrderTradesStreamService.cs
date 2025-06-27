using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.MarketData.Contracts;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.PortfolioManager.Contracts.Interfaces;
using Vertr.TinvestGateway.Application.Converters;
using Vertr.TinvestGateway.Application.Settings;

namespace Vertr.TinvestGateway.Application.BackgroundServices;

public class OrderTradesStreamService : StreamServiceBase
{
    private readonly IPortfolioRepository _portfolioRepository;

    protected override bool IsEnabled => TinvestSettings.OrderTradesStreamEnabled;

    public OrderTradesStreamService(
        IPortfolioRepository portfolioRepository,
        IOptions<TinvestSettings> tinvestOptions,
        InvestApiClient investApiClient,
        IMediator mediator,
        ILogger<OrderTradesStreamService> logger) :
            base(tinvestOptions, investApiClient, mediator, logger)
    {
        _portfolioRepository = portfolioRepository;
    }

    protected override async Task Subscribe(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken stoppingToken = default)
    {
        var accounts = _portfolioRepository.GetActiveAccounts();
        var request = new Tinkoff.InvestApi.V1.TradesStreamRequest();
        request.Accounts.Add(accounts);

        using var stream = InvestApiClient.OrdersStream.TradesStream(request, headers: null, deadline, stoppingToken);

        await foreach (var response in stream.ResponseStream.ReadAllAsync(stoppingToken))
        {
            if (response.PayloadCase == Tinkoff.InvestApi.V1.TradesStreamResponse.PayloadOneofCase.OrderTrades)
            {
                var orderTradesRequest = new OrderTradesRequest
                {
                    OrderTrades = response.OrderTrades.Convert(),
                    InstrumentIdentity = new InstrumentIdentity(Guid.Parse(response.OrderTrades.InstrumentUid)),
                };

                logger.LogInformation($"New order trades received for OrderId={response.OrderTrades.OrderId}");

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
