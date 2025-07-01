using Grpc.Core;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.PortfolioManager.Contracts.Interfaces;
using Vertr.TinvestGateway.Application.Converters;
using Vertr.TinvestGateway.Application.Settings;

namespace Vertr.TinvestGateway.Application.BackgroundServices;

public class OrderTradesStreamService : StreamServiceBase
{
    protected override bool IsEnabled => TinvestSettings.OrderTradesStreamEnabled;

    public OrderTradesStreamService(
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
        var staticMarketDataProvider = scope.ServiceProvider.GetRequiredService<IStaticMarketDataProvider>();

        var accounts = portfolioRepository.GetActiveAccounts();
        var request = new Tinkoff.InvestApi.V1.TradesStreamRequest();
        request.Accounts.Add(accounts);

        using var stream = investApiClient.OrdersStream.TradesStream(request, headers: null, deadline, stoppingToken);

        await foreach (var response in stream.ResponseStream.ReadAllAsync(stoppingToken))
        {
            if (response.PayloadCase == Tinkoff.InvestApi.V1.TradesStreamResponse.PayloadOneofCase.OrderTrades)
            {
                var instrumentId = Guid.Parse(response.OrderTrades.InstrumentUid);
                var currency = await staticMarketDataProvider.GetInstrumentCurrency(instrumentId);

                var orderTradesRequest = new OrderTradesRequest
                {
                    InstrumentId = instrumentId,
                    OrderTrades = response.OrderTrades.Convert(currency),
                };

                logger.LogInformation($"New order trades received for OrderId={response.OrderTrades.OrderId}");

                await mediator.Send(orderTradesRequest, stoppingToken);
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
