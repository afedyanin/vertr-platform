using Google.Protobuf.WellKnownTypes;
using Tinkoff.InvestApi;
using Vertr.TinvestGateway.Application.Converters;
using Vertr.TinvestGateway.Contracts;
using Vertr.TinvestGateway.Contracts.Enums;
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
    
    public async Task<Instrument?> GetInstrumentByTicker(string ticker, string classCode)
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
        string ticker, 
        string classCode, 
        CandleInterval interval, DateTime from, DateTime to, int? limit)
    {
        var instrumentRequest = new Tinkoff.InvestApi.V1.InstrumentRequest
        {
            ClassCode = classCode,
            Id = ticker,
            IdType = Tinkoff.InvestApi.V1.InstrumentIdType.Ticker,
        };

        var instrumentResponse = await InvestApiClient.Instruments.GetInstrumentByAsync(instrumentRequest);
        var instrumentId = instrumentResponse?.Instrument.Uid;

        if (string.IsNullOrEmpty(instrumentId))
        {
            return [];
        }

        var request = new Tinkoff.InvestApi.V1.GetCandlesRequest
        {
            From = Timestamp.FromDateTime(from.ToUniversalTime()),
            To = Timestamp.FromDateTime(to.ToUniversalTime()),
            InstrumentId = instrumentId,
            Interval = interval.Convert(),
        };

        if (limit.HasValue)
        {
            request.Limit = limit.Value;
        }

        var response = await InvestApiClient.MarketData.GetCandlesAsync(request);
        var candles = response.Candles.ToArray().Convert(instrumentId);

        return candles;
    }
}
