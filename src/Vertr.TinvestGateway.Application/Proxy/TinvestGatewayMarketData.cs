using Google.Protobuf.WellKnownTypes;
using Tinkoff.InvestApi;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.TinvestGateway.Application.Converters;

namespace Vertr.TinvestGateway.Application.Proxy;

internal class TinvestGatewayMarketData : TinvestGatewayBase, IMarketDataGateway
{
    public TinvestGatewayMarketData(InvestApiClient investApiClient) : base(investApiClient)
    {
    }

    public async Task<Instrument[]?> FindInstrument(string query)
    {
        var request = new Tinkoff.InvestApi.V1.FindInstrumentRequest
        {
            Query = query
        };

        var response = await InvestApiClient.Instruments.FindInstrumentAsync(request);
        var instruments = response.Instruments.ToArray().ToInstruments();

        return instruments;
    }

    public async Task<Instrument?> GetInstrument(Guid instrumentId)
    {
        var request = new Tinkoff.InvestApi.V1.InstrumentRequest
        {
            Id = instrumentId.ToString(),
            IdType = Tinkoff.InvestApi.V1.InstrumentIdType.Uid,
        };

        var response = await InvestApiClient.Instruments.GetInstrumentByAsync(request);

        if (response == null || response.Instrument == null)
        {
            return null;
        }

        var instrument = response.Instrument.ToInstrument();

        return instrument;
    }

    public async Task<Instrument?> GetInstrument(Symbol symbol)
    {
        var request = new Tinkoff.InvestApi.V1.InstrumentRequest
        {
            ClassCode = symbol.ClassCode,
            Id = symbol.Ticker,
            IdType = Tinkoff.InvestApi.V1.InstrumentIdType.Ticker,
        };

        var response = await InvestApiClient.Instruments.GetInstrumentByAsync(request);

        if (response == null || response.Instrument == null)
        {
            return null;
        }

        var instrument = response.Instrument.ToInstrument();

        return instrument;
    }

    public async Task<Candle[]?> GetCandles(
        Symbol symbol,
        CandleInterval interval,
        DateTime from,
        DateTime to,
        int? limit)
    {
        var instrument = await GetInstrument(symbol);

        if (instrument == null)
        {
            return [];
        }

        var request = new Tinkoff.InvestApi.V1.GetCandlesRequest
        {
            From = Timestamp.FromDateTime(from.ToUniversalTime()),
            To = Timestamp.FromDateTime(to.ToUniversalTime()),
            InstrumentId = instrument.Id.ToString(),
            Interval = interval.Convert(),
        };

        if (limit.HasValue)
        {
            request.Limit = limit.Value;
        }

        var response = await InvestApiClient.MarketData.GetCandlesAsync(request);
        var candles = response.Candles.ToArray().Convert(instrument.Id);

        return candles;
    }
}
