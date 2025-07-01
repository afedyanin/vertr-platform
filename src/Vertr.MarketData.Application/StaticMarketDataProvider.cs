using System.Globalization;
using Microsoft.Extensions.Options;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.MarketData.Application;
internal class StaticMarketDataProvider : IStaticMarketDataProvider
{
    private readonly MarketDataSettings _settings;
    private readonly IMarketDataGateway _gateway;

    private readonly Dictionary<Guid, Instrument> _instrumentsById = [];
    private readonly Dictionary<Symbol, Instrument> _instruments = [];
    private readonly Dictionary<Guid, CandleInterval> _intervals = [];
    private readonly List<CandleSubscription> _subscriptions = [];

    private bool _isInitialized = false;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);


    public StaticMarketDataProvider(
        IOptions<MarketDataSettings> options,
        IMarketDataGateway gateway)
    {
        _settings = options.Value;
        _gateway = gateway;
        _subscriptions = [.. GetCandleSubscriptions()];

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

    public Task<Instrument?> GetInstrumentById(Guid instrumentId)
    {
        _instrumentsById.TryGetValue(instrumentId, out var instrumentById);
        return Task.FromResult(instrumentById);
    }

    public Task<Instrument?> GetInstrument(Symbol symbol)
    {
        _instruments.TryGetValue(symbol, out var instrumentByTicker);
        return Task.FromResult(instrumentByTicker);
    }

    public Task<Instrument[]> GetInstruments()
        => Task.FromResult(_instrumentsById.Values.ToArray());

    public Task<CandleSubscription[]> GetSubscriptions()
        => Task.FromResult(_subscriptions.ToArray());

    private void LoadIntervals()
    {
        _intervals.Clear();

        foreach (var item in _subscriptions)
        {
            _intervals[item.InstrumentId] = item.Interval;
        }
    }

    private async Task Loadnstruments()
    {
        _instrumentsById.Clear();
        _instruments.Clear();

        foreach (var instrumentId in _settings.Instruments)
        {
            var instrument = await _gateway.GetInstrument(instrumentId);
            if (instrument != null)
            {
                _instrumentsById[instrument.Id] = instrument;
                _instruments[instrument.Symbol] = instrument;
            }
        }

        foreach (var subscription in _subscriptions)
        {
            var instrument = await _gateway.GetInstrument(subscription.InstrumentId);

            if (instrument != null)
            {
                _instrumentsById[instrument.Id] = instrument;
                _instruments[instrument.Symbol] = instrument;
            }
        }
    }

    public async Task<Instrument[]> FindInstrument(string query)
    {
        var instruments = await _gateway.FindInstrument(query);

        return instruments?.ToArray() ?? [];
    }

    private CandleSubscription[] GetCandleSubscriptions()
    {
        var res = new List<CandleSubscription>();

        foreach (var kvp in _settings.Subscriptions)
        {
            res.Add(new CandleSubscription
            {
                InstrumentId = kvp.Key,
                Interval = kvp.Value
            });

        }

        return [.. res];
    }

    public Guid? GetCurrencyId(string currencyCode)
    {
        if (_settings.Currencies.TryGetValue(
            currencyCode.ToUpper(CultureInfo.InvariantCulture),
            out var currencyId))
        {
            return currencyId;
        }

        return null;
    }

    public async Task<Guid?> GetInstrumentCurrencyId(Guid instrumentId)
    {
        var instrument = await GetInstrumentById(instrumentId);

        if (instrument == null || instrument.Currency == null)
        {
            return null;
        }

        return GetCurrencyId(instrument.Currency);
    }
}
