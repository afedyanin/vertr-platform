using Grpc.Core;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.TinvestGateway.Abstractions;
using Vertr.TinvestGateway.Converters;
using Vertr.TinvestGateway.Repositories;

namespace Vertr.TinvestGateway.Host.BackgroundServices;

public class OrderStateStreamService : StreamServiceBase
{
    protected override bool IsEnabled => TinvestSettings.OrderStateStreamEnabled;

    public OrderStateStreamService(
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
        var orderStateRepository = scope.ServiceProvider.GetRequiredService<IOrderStateRepository>();
        var portfolioService = scope.ServiceProvider.GetRequiredService<IPortfolioService>();

        var request = new Tinkoff.InvestApi.V1.OrderStateStreamRequest();
        var accountId = TinvestSettings.AccountId;
        request.Accounts.Add(accountId);

        using var stream = investApiClient.OrdersStream.OrderStateStream(request, headers: null, deadline, stoppingToken);

        await foreach (var response in stream.ResponseStream.ReadAllAsync(stoppingToken))
        {
            if (response.PayloadCase == Tinkoff.InvestApi.V1.OrderStateStreamResponse.PayloadOneofCase.OrderState)
            {
                //var json = JsonSerializer.Serialize(response.OrderState);
                //logger.LogInformation($"New order state received for AccountId={accountId} State:{json}");
                logger.LogInformation($"New order state received for AccountId={accountId}");

                var orderState = response.OrderState.Convert(accountId);
                await orderStateRepository.Save(orderState);
                await portfolioService.BindOrderToPortfolio(orderState);
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