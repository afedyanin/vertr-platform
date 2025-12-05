using Grpc.Core;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.TinvestGateway.Abstractions;
using Vertr.TinvestGateway.Converters;
using Vertr.TinvestGateway.Repositories;

namespace Vertr.TinvestGateway.Host.BackgroundServices;

public class OrderTradesStreamService : StreamServiceBase
{
    protected override bool IsEnabled => TinvestSettings.OrderTradesStreamEnabled;

    public OrderTradesStreamService(
        IServiceProvider serviceProvider,
        IOptions<TinvestSettings> tinvestOptions,
        ILogger<OrderTradesStreamService> logger) : base(serviceProvider, tinvestOptions, logger)
    {
    }

    protected override async Task Subscribe(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken stoppingToken = default)
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var investApiClient = scope.ServiceProvider.GetRequiredService<InvestApiClient>();
        var orderTradeRepository = scope.ServiceProvider.GetRequiredService<IOrderTradeRepository>();
        var portfolioService = scope.ServiceProvider.GetRequiredService<IPortfolioService>();

        var request = new Tinkoff.InvestApi.V1.TradesStreamRequest();
        request.Accounts.Add(TinvestSettings.AccountId);

        using var stream = investApiClient.OrdersStream.TradesStream(request, headers: null, deadline, stoppingToken);

        await foreach (var response in stream.ResponseStream.ReadAllAsync(stoppingToken))
        {
            if (response.PayloadCase == Tinkoff.InvestApi.V1.TradesStreamResponse.PayloadOneofCase.OrderTrades)
            {
                var orderTrades = response.OrderTrades.Convert();
                await orderTradeRepository.Save(orderTrades);
                await portfolioService.Update(orderTrades);

                logger.LogInformation($"New order trades received for OrderId={response.OrderTrades.OrderId}");
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