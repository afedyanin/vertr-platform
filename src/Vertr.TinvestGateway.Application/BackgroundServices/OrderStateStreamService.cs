using Grpc.Core;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.PortfolioManager.Contracts.Interfaces;
using Vertr.TinvestGateway.Application.Converters;
using Vertr.TinvestGateway.Application.Settings;

namespace Vertr.TinvestGateway.Application.BackgroundServices;

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
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var portfolioRepository = scope.ServiceProvider.GetRequiredService<IPortfolioRepository>();
        var investApiClient = scope.ServiceProvider.GetRequiredService<InvestApiClient>();

        var accounts = portfolioRepository.GetActiveAccounts();
        var request = new Tinkoff.InvestApi.V1.OrderStateStreamRequest();
        request.Accounts.Add(accounts);

        using var stream = investApiClient.OrdersStream.OrderStateStream(request, headers: null, deadline, stoppingToken);

        await foreach (var response in stream.ResponseStream.ReadAllAsync(stoppingToken))
        {
            if (response.PayloadCase == Tinkoff.InvestApi.V1.OrderStateStreamResponse.PayloadOneofCase.OrderState)
            {
                var orderStateRequest = new OrderStateRequest
                {
                    OrderState = response.OrderState.Convert(),
                    AccountId = response.OrderState.AccountId,
                };

                logger.LogInformation($"New order state received for AccountId={response.OrderState.AccountId}");

                await mediator.Send(orderStateRequest, stoppingToken);
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
