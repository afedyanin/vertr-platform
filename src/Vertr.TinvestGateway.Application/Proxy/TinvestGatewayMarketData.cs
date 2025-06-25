using Google.Protobuf.WellKnownTypes;
using Tinkoff.InvestApi;
using Vertr.MarketData.Contracts;
using Vertr.TinvestGateway.Application.Converters;
using Vertr.TinvestGateway.Contracts.Interfaces;

namespace Vertr.TinvestGateway.Application.Proxy;

internal class TinvestGatewayMarketData : TinvestGatewayBase, ITinvestGatewayMarketData
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

    public async Task<Instrument?> GetInstrumentByTicker(string classCode, string ticker)
    {
        var request = new Tinkoff.InvestApi.V1.InstrumentRequest
        {
            ClassCode = classCode,
            Id = ticker,
            IdType = Tinkoff.InvestApi.V1.InstrumentIdType.Ticker,
        };

        var response = await InvestApiClient.Instruments.GetInstrumentByAsync(request);
        var instrument = response.Instrument.ToInstrument();

        return instrument;
    }
    public async Task<Instrument?> GetInstrumentById(string instumentId)
    {
        var request = new Tinkoff.InvestApi.V1.InstrumentRequest
        {
            Id = instumentId,
            IdType = Tinkoff.InvestApi.V1.InstrumentIdType.Uid,
        };

        var response = await InvestApiClient.Instruments.GetInstrumentByAsync(request);
        var instrument = response.Instrument.ToInstrument();

        return instrument;
    }

    public async Task<Candle[]?> GetCandles(
        InstrumentIdentity instrumentIdentity,
        CandleInterval interval,
        DateTime from,
        DateTime to,
        int? limit)
    {
        var instrument = instrumentIdentity.InstrumentId.HasValue ?
            await GetInstrumentById(instrumentIdentity.InstrumentId.Value.ToString()) :
            await GetInstrumentByTicker(instrumentIdentity.ClassCode, instrumentIdentity.Ticker);

        if (instrument == null || !instrument.InstrumentIdentity.InstrumentId.HasValue)
        {
            return [];
        }

        var request = new Tinkoff.InvestApi.V1.GetCandlesRequest
        {
            From = Timestamp.FromDateTime(from.ToUniversalTime()),
            To = Timestamp.FromDateTime(to.ToUniversalTime()),
            InstrumentId = instrument.InstrumentIdentity.InstrumentId.Value.ToString(),
            Interval = interval.Convert(),
        };

        if (limit.HasValue)
        {
            request.Limit = limit.Value;
        }

        var response = await InvestApiClient.MarketData.GetCandlesAsync(request);
        var candles = response.Candles.ToArray().Convert();

        return candles;
    }
}
