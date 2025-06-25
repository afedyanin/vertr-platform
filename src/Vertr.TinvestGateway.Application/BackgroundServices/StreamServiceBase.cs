using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.MarketData.Contracts.Requests;
using Vertr.TinvestGateway.Application.Settings;
using Vertr.TinvestGateway.Contracts.Interfaces;

namespace Vertr.TinvestGateway.Application.BackgroundServices;

public abstract class StreamServiceBase : BackgroundService
{
    protected InvestApiClient InvestApiClient { get; private set; }
    protected TinvestSettings TinvestSettings { get; private set; }

    protected IMediator Mediator { get; private set; }

    protected abstract bool IsEnabled { get; }

    protected ILogger Logger { get; private set; }

    private readonly string _serviceName;

    private readonly Dictionary<string, Instrument> _instruments = [];
    private readonly Dictionary<string, CandleInterval> _intervals = [];

    private readonly IMarketDataService _marketDataService;
    private readonly ITinvestGatewayMarketData _tinvestGatewayMarketData;

    protected StreamServiceBase(
        IOptions<TinvestSettings> tinvestOptions,
        InvestApiClient investApiClient,
        ITinvestGatewayMarketData tinvestGatewayMarketData,
        IMarketDataService marketDataService,
        IMediator mediator,
        ILogger logger)
    {
        TinvestSettings = tinvestOptions.Value;
        InvestApiClient = investApiClient;
        Logger = logger;
        Mediator = mediator;

        _serviceName = GetType().Name;
        _tinvestGatewayMarketData = tinvestGatewayMarketData;
        _marketDataService = marketDataService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            if (!IsEnabled)
            {
                Logger.LogWarning($"{_serviceName} is disabled.");
                return;
            }

            await OnBeforeStart(stoppingToken);
            await StartConsumingLoop(stoppingToken);

            Logger.LogInformation($"{_serviceName} execution completed at {DateTime.UtcNow:O}");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }
    }

    protected virtual async Task OnBeforeStart(CancellationToken stoppingToken)
    {
        _instruments.Clear();

        var subscriptions = await _marketDataService.GetSubscriptions();

        foreach (var subscription in subscriptions)
        {
            var instrument = await _tinvestGatewayMarketData.GetInstrumentByTicker(subscription.Ticker, subscription.ClassCode);

            if (instrument != null)
            {
                _instruments[instrument.Id!] = instrument;
                _intervals[instrument.Id!] = subscription.Interval;
            }
        }

        if (_instruments.Count != 0)
        {
            var request = new InstrumentSnapshotReceived
            {
                Instruments = [.. _instruments.Values]
            };

            await Mediator.Send(request, stoppingToken);
        }
    }

    private async Task StartConsumingLoop(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Logger.LogInformation($"{_serviceName} started at {DateTime.UtcNow:O}");
                await Subscribe(Logger, deadline: null, stoppingToken);
            }
            catch (RpcException rpcEx)
            {
                if (rpcEx.StatusCode != StatusCode.DeadlineExceeded)
                {
                    Logger.LogError(rpcEx, $"{_serviceName} consuming exception. Message={rpcEx.Message}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"{_serviceName} consuming exception. Message={ex.Message}");
            }
        }
    }

    protected abstract Task Subscribe(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken stoppingToken = default);
}
