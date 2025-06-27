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

    public async Task<Instrument?> GetInstrument(InstrumentIdentity instrumentIdentity)
    {
        var request = instrumentIdentity.HasUid ?
            new Tinkoff.InvestApi.V1.InstrumentRequest
            {
                Id = instrumentIdentity.Id.ToString(),
                IdType = Tinkoff.InvestApi.V1.InstrumentIdType.Uid,
            } :
            new Tinkoff.InvestApi.V1.InstrumentRequest
            {
                ClassCode = instrumentIdentity.ClassCode,
                Id = instrumentIdentity.Ticker,
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
        InstrumentIdentity instrumentIdentity,
        CandleInterval interval,
        DateTime from,
        DateTime to,
        int? limit)
    {
        var instrument = await GetInstrument(instrumentIdentity);

        if (instrument == null)
        {
            return [];
        }

        var request = new Tinkoff.InvestApi.V1.GetCandlesRequest
        {
            From = Timestamp.FromDateTime(from.ToUniversalTime()),
            To = Timestamp.FromDateTime(to.ToUniversalTime()),
            InstrumentId = instrument.InstrumentIdentity.Id.ToString(),
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
