using Microsoft.Extensions.Options;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.MarketData.Application;
internal class StaticMarketDataProvider : IStaticMarketDataProvider
{
    private readonly MarketDataSettings _settings;
    private readonly IMarketDataGateway _gateway;

    private readonly Dictionary<Guid, Instrument> _instrumentsById = [];
    private readonly Dictionary<string, Instrument> _instrumentsByTicker = [];
    private readonly Dictionary<Guid, CandleInterval> _intervalsById = [];
    private readonly List<CandleSubscription> _subscriptions = [];

    private bool _isInitialized = false;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);


    public StaticMarketDataProvider(
        IOptions<MarketDataSettings> options,
        IMarketDataGateway gateway)
    {
        _settings = options.Value;
        _gateway = gateway;
        _subscriptions = [.. _settings.GetCandleSubscriptions()];

    }
    public async Task InitialLoad()
    {
        if (_isInitialized)
        {
            return;
        }

        _semaphore.Wait();

        try
        {
            if (_isInitialized)
            {
                return;
            }

            LoadIntervals();
            await Loadnstruments();
            _isInitialized = true;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public Task<Instrument?> GetInstrument(InstrumentIdentity instrumentIdentity)
    {
        if (instrumentIdentity.HasUid &&
            _instrumentsById.TryGetValue(instrumentIdentity.Id, out var instrumentById))
        {
            return Task.FromResult<Instrument?>(instrumentById);
        }

        _instrumentsByTicker.TryGetValue(instrumentIdentity.Symbol, out var instrumentByTicker);

        return Task.FromResult(instrumentByTicker);
    }

    public Task<Instrument[]> GetInstruments()
        => Task.FromResult(_instrumentsById.Values.ToArray());

    public Task<CandleInterval> GetInterval(InstrumentIdentity instrumentIdentity)
    {
        _intervalsById.TryGetValue(instrumentIdentity.Id, out var interval);

        return Task.FromResult(interval);
    }

    public Task<CandleSubscription[]> GetSubscriptions()
        => Task.FromResult(_subscriptions.ToArray());

    private void LoadIntervals()
    {
        _intervalsById.Clear();

        foreach (var item in _subscriptions)
        {
            if (item.InstrumentIdentity != null &&
                item.InstrumentIdentity.HasUid)
            {
                _intervalsById[item.InstrumentIdentity.Id] = item.Interval;
            }
        }
    }

    private async Task Loadnstruments()
    {
        _instrumentsById.Clear();
        _instrumentsByTicker.Clear();

        foreach (var instrumentId in _settings.Instruments)
        {
            var instrument = await _gateway.GetInstrument(new InstrumentIdentity(instrumentId));
            if (instrument != null)
            {
                var uid = instrument.InstrumentIdentity?.Id ?? Guid.Empty;
                _instrumentsById[uid] = instrument;
                _instrumentsByTicker[instrument.InstrumentIdentity?.Symbol ?? ""] = instrument;
            }
        }

        foreach (var subscription in _subscriptions)
        {
            var instrument = await _gateway.GetInstrument(subscription.InstrumentIdentity);

            if (instrument != null)
            {
                var uid = instrument.InstrumentIdentity?.Id ?? Guid.Empty;
                _instrumentsById[uid] = instrument;
                _instrumentsByTicker[instrument.InstrumentIdentity?.Symbol ?? ""] = instrument;
            }
        }
    }

    public async Task<Instrument[]> FindInstrument(string query)
    {
        var instruments = await _gateway.FindInstrument(query);

        return instruments?.ToArray() ?? [];
    }
}
