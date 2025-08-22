using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts;
using Vertr.Platform.Common.Channels;
using Vertr.PortfolioManager.Contracts.Interfaces;
using Vertr.TinvestGateway.Converters;

namespace Vertr.TinvestGateway.BackgroundServices;

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
        var portfolioRepository = scope.ServiceProvider.GetRequiredService<IPortfolioRepository>();
        var investApiClient = scope.ServiceProvider.GetRequiredService<InvestApiClient>();
        var currencyRepository = scope.ServiceProvider.GetRequiredService<ICurrencyRepository>();
        var orderTradesProducer = scope.ServiceProvider.GetRequiredService<IDataProducer<OrderTrades>>();

        var request = new Tinkoff.InvestApi.V1.TradesStreamRequest();
        request.Accounts.Add(TinvestSettings.AccountId);

        using var stream = investApiClient.OrdersStream.TradesStream(request, headers: null, deadline, stoppingToken);

        await foreach (var response in stream.ResponseStream.ReadAllAsync(stoppingToken))
        {
            if (response.PayloadCase == Tinkoff.InvestApi.V1.TradesStreamResponse.PayloadOneofCase.OrderTrades)
            {
                var instrumentId = Guid.Parse(response.OrderTrades.InstrumentUid);
                var currency = await currencyRepository.GetInstrumentCurrency(instrumentId);
                var orderTradesRequest = response.OrderTrades.Convert(currency);

                logger.LogInformation($"New order trades received for OrderId={response.OrderTrades.OrderId}");

                await orderTradesProducer.Produce(orderTradesRequest, stoppingToken);
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
