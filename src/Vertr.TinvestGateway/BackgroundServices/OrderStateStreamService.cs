using System.Text.Json;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.OrderExecution.Contracts;
using Vertr.Platform.Common.Channels;
using Vertr.PortfolioManager.Contracts.Interfaces;
using Vertr.TinvestGateway.Converters;

namespace Vertr.TinvestGateway.BackgroundServices;

public class OrderStateStreamService : StreamServiceBase
{
    protected override bool IsEnabled => TinvestSettings.OrderStateStreamEnabled;

    public OrderStateStreamService(
        IServiceProvider serviceProvider,
        IOptions<TinvestSettings> tinvestOptions,
        ILogger<OrderTradesStreamService> logger) :
            base(serviceProvider, tinvestOptions, logger)
    {
    }

    protected override async Task Subscribe(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken stoppingToken = default)
    {
        using var scope = ServiceProvider.CreateScope();
        var portfolioRepository = scope.ServiceProvider.GetRequiredService<IPortfolioProvider>();
        var investApiClient = scope.ServiceProvider.GetRequiredService<InvestApiClient>();
        var orderStateProducer = scope.ServiceProvider.GetRequiredService<IDataProducer<OrderState>>();

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
                await orderStateProducer.Produce(orderState, stoppingToken);
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
