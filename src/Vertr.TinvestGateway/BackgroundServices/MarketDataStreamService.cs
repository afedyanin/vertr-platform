using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Commands;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.Platform.Common.Channels;
using Vertr.Platform.Common.Jobs;
using Vertr.TinvestGateway.Converters;

namespace Vertr.TinvestGateway.BackgroundServices;

public class MarketDataStreamService : StreamServiceBase
{
    private readonly IDataConsumer<CandleSubscription> _candleSubscriptionsConsumer;

    private CancellationTokenSource _restartSubscriptionToken;

    protected override bool IsEnabled => TinvestSettings.MarketDataStreamEnabled;

    public MarketDataStreamService(
        IDataConsumer<CandleSubscription> candleSubscriptionsConsumer,
        IServiceProvider serviceProvider,
        IOptions<TinvestSettings> tinvestOptions,
        ILogger<OrderTradesStreamService> logger) :
            base(serviceProvider, tinvestOptions, logger)
    {
        _candleSubscriptionsConsumer = candleSubscriptionsConsumer;
    }

    protected override Task OnBeforeStart(CancellationToken stoppingToken)
    {
        var consumingTask = _candleSubscriptionsConsumer.Consume(OnCandleSubscriptionUpdate, stoppingToken);
        return base.OnBeforeStart(stoppingToken);
    }

    protected override async Task StartConsumingLoop(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _restartSubscriptionToken = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

            try
            {
                Logger.LogInformation($"{nameof(MarketDataStreamService)} started at {DateTime.UtcNow:O}");
                await Subscribe(Logger, deadline: null, _restartSubscriptionToken.Token);
            }
            catch (RpcException rpcEx)
            {
                if (rpcEx.StatusCode != StatusCode.DeadlineExceeded)
                {
                    Logger.LogError(rpcEx, $"{nameof(MarketDataStreamService)} consuming exception. Message={rpcEx.Message}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"{nameof(MarketDataStreamService)} consuming exception. Message={ex.Message}");
            }
        }
    }

    protected override async Task Subscribe(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken stoppingToken = default)
    {
        using var scope = ServiceProvider.CreateScope();
        var staticMarketDataProvider = scope.ServiceProvider.GetRequiredService<IInstrumentsRepository>();
        var investApiClient = scope.ServiceProvider.GetRequiredService<InvestApiClient>();
        var marketDataProducer = scope.ServiceProvider.GetRequiredService<IDataProducer<Candle>>();
        var candlesRepository = scope.ServiceProvider.GetRequiredService<ICandlesRepository>();
        var subscriptionsRepository = scope.ServiceProvider.GetRequiredService<ISubscriptionsRepository>();
        var jobScheduler = scope.ServiceProvider.GetRequiredService<IJobScheduler>();

        var candleRequest = new Tinkoff.InvestApi.V1.SubscribeCandlesRequest
        {
            SubscriptionAction = Tinkoff.InvestApi.V1.SubscriptionAction.Subscribe,
            WaitingClose = TinvestSettings.WaitCandleClose,
        };

        var subscriptions = await subscriptionsRepository.GetAll();

        foreach (var sub in subscriptions)
        {
            if (sub.Disabled)
            {
                continue;
            }

            logger.LogInformation($"Adding candle subscription: Id={sub.Id} InstrumentId={sub.InstrumentId}");

            candleRequest.Instruments.Add(new Tinkoff.InvestApi.V1.CandleInstrument()
            {
                InstrumentId = sub.InstrumentId.ToString(),
                Interval = sub.Interval.ConvertToSubscriptionInterval()
            });
        }

        var request = new Tinkoff.InvestApi.V1.MarketDataServerSideStreamRequest()
        {
            SubscribeCandlesRequest = candleRequest,
        };

        using var stream = investApiClient.MarketDataStream.MarketDataServerSideStream(request, headers: null, deadline, stoppingToken);

        EnqueueMarketDataJobs(jobScheduler, stoppingToken);

        await foreach (var response in stream.ResponseStream.ReadAllAsync(stoppingToken))
        {
            if (response.PayloadCase == Tinkoff.InvestApi.V1.MarketDataResponse.PayloadOneofCase.Candle)
            {
                var instrumentId = response.Candle.InstrumentUid;
                var candle = response.Candle.Convert(Guid.Parse(instrumentId));

                await candlesRepository.Upsert([candle]);
                await marketDataProducer.Produce(candle, stoppingToken);
            }
            else if (response.PayloadCase == Tinkoff.InvestApi.V1.MarketDataResponse.PayloadOneofCase.SubscribeCandlesResponse)
            {
                var subs = response.SubscribeCandlesResponse;
                var all = subs.CandlesSubscriptions.ToArray();
                await UpdateSubscriptions(subscriptionsRepository, all);

                logger.LogInformation($"Candle subscriptions received: TrackingId={subs.TrackingId} Details={string.Join(',',
                    [.. all.Select(s => $"Id={s.SubscriptionId} Status={s.SubscriptionStatus} Instrument={s.InstrumentUid} Inverval={s.Interval}")])}");
            }
            else if (response.PayloadCase == Tinkoff.InvestApi.V1.MarketDataResponse.PayloadOneofCase.Ping)
            {
                logger.LogDebug($"Candle ping received: {response.Ping}");
            }
        }
    }

    private async Task UpdateSubscriptions(ISubscriptionsRepository repo, Tinkoff.InvestApi.V1.CandleSubscription[] subscriptions)
    {
        foreach (var subscription in subscriptions)
        {
            if (subscription == null)
            {
                continue;
            }

            var found = await repo.Find(Guid.Parse(subscription.InstrumentUid), subscription.Interval.Convert());

            if (found == null)
            {
                continue;
            }

            found.ExternalSubscriptionId = subscription.SubscriptionId;
            found.ExternalStatus = subscription.SubscriptionStatus.ToString();

            await repo.Save(found);
        }
    }

    private void EnqueueMarketDataJobs(IJobScheduler jobScheduler, CancellationToken token)
    {
        _ = jobScheduler.Schedule(new LoadIntradayCandlesRequest(), TimeSpan.FromSeconds(30), token);
        _ = jobScheduler.Schedule(new LoadHistoryCandlesRequest(), TimeSpan.FromMinutes(1), token);
        _ = jobScheduler.Schedule(new CleanIntradayCandlesRequest(), TimeSpan.FromMinutes(1), token);
    }

    private Task OnCandleSubscriptionUpdate(
        CandleSubscription candleSubscription,
        CancellationToken stoppingToken)
    {
        try
        {
            Logger.LogDebug($"CandleSubscription update received. SubscriptionId={candleSubscription.Id}");
            _restartSubscriptionToken?.Cancel();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"CandleSubscription update exception. Message={ex.Message}");
        }

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        base.Dispose();

        _restartSubscriptionToken?.Dispose();
    }
}
