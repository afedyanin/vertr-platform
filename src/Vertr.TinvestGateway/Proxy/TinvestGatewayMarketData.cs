using Google.Protobuf.WellKnownTypes;
using Tinkoff.InvestApi;
using Vertr.TinvestGateway.Abstractions;
using Vertr.TinvestGateway.Contracts.MarketData;
using Vertr.TinvestGateway.Converters;

namespace Vertr.TinvestGateway.Proxy;

internal class TinvestGatewayMarketData : TinvestGatewayBase, IMarketDataGateway
{
    public TinvestGatewayMarketData(InvestApiClient investApiClient) : base(investApiClient)
    {
    }

    public async Task<Instrument[]?> FindInstrument(string query)
    {
        var request = new Tinkoff.InvestApi.V1.FindInstrumentRequest
        {
            Query = query,
            ApiTradeAvailableFlag = true,
        };

        var response = await InvestApiClient.Instruments.FindInstrumentAsync(request);
        var instruments = response.Instruments.ToArray().ToInstruments();

        return instruments;
    }

    public async Task<Instrument?> GetInstrumentById(Guid instrumentId)
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

    public async Task<Instrument?> GetInstrumentBySymbol(string classCode, string ticker)
    {
        var request = new Tinkoff.InvestApi.V1.InstrumentRequest
        {
            ClassCode = classCode,
            Id = ticker,
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
        Guid instrumentId,
        DateOnly? date = null,
        CandleInterval interval = CandleInterval.Min_1)
    {
        date ??= DateOnly.FromDateTime(DateTime.UtcNow);
        var from = date.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var to = date.Value.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        var request = new Tinkoff.InvestApi.V1.GetCandlesRequest
        {
            From = Timestamp.FromDateTime(from.ToUniversalTime()),
            To = Timestamp.FromDateTime(to.ToUniversalTime()),
            InstrumentId = instrumentId.ToString(),
            Interval = interval.Convert(),
        };

        var response = await InvestApiClient.MarketData.GetCandlesAsync(request);
        var candles = response.Candles.ToArray().Convert(instrumentId);

        return candles;
    }
}