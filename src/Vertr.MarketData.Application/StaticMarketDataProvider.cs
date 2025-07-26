using System.Globalization;
using Microsoft.Extensions.Options;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.MarketData.Application;
internal class StaticMarketDataProvider : IMarketInstrumentRepository
{
    private readonly MarketDataSettings _settings;
    private readonly IMarketDataGateway _gateway;

    private readonly Dictionary<Guid, Instrument> _instrumentsById = [];
    private readonly Dictionary<Symbol, Instrument> _instruments = [];

    public StaticMarketDataProvider(
        IOptions<MarketDataSettings> options,
        IMarketDataGateway gateway)
    {
        _settings = options.Value;
        _gateway = gateway;
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

    private async Task Loadnstruments()
    {
        _instrumentsById.Clear();
        _instruments.Clear();

        foreach (var item in _settings.Currencies.Values)
        {
            var instrument = await _gateway.GetInstrument(item);
            if (instrument != null)
            {
                _instrumentsById[instrument.Id] = instrument;
                _instruments[instrument.Symbol] = instrument;
            }
        }

        foreach (var instrumentId in _settings.Instruments)
        {
            var instrument = await _gateway.GetInstrument(instrumentId);
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

    public async Task<string> GetInstrumentCurrency(Guid instrumentId)
    {
        var instrument = await GetInstrumentById(instrumentId);

        if (instrument == null || instrument.Currency == null)
        {
            return string.Empty;
        }

        return instrument.Currency ?? string.Empty;
    }
}
