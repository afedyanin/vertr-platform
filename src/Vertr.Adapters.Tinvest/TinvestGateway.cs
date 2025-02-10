using Tinkoff.InvestApi;
using Vertr.Domain.Ports;
using Vertr.Adapters.Tinvest.Converters;
using Vertr.Domain;
using Microsoft.Extensions.Options;

namespace Vertr.Adapters.Tinvest;

internal class TinvestGateway : ITinvestGateway
{
    private readonly InvestApiClient _investApiClient;
    private readonly TinvestSettings _investConfiguration;

    public TinvestGateway(
        InvestApiClient investApiClient,
        IOptions<TinvestSettings> options)
    {
        _investApiClient = investApiClient;
        _investConfiguration = options.Value;
    }

    public async Task<IEnumerable<Instrument>> FindInstrument(string query)
    {
        var request = new Tinkoff.InvestApi.V1.FindInstrumentRequest
        {
            Query = query
        };

        var response = await _investApiClient.Instruments.FindInstrumentAsync(request);

        return response.Instruments.Convert();
    }

    public async Task<Instrument> GetInstrument(string ticker, string classCode)
    {
        var request = new Tinkoff.InvestApi.V1.InstrumentRequest
        {
            ClassCode = classCode,
            Id = ticker,
            IdType = Tinkoff.InvestApi.V1.InstrumentIdType.Ticker,
        };

        var response = await _investApiClient.Instruments.GetInstrumentByAsync(request);

        return response.Instrument.Convert();
    }

    public Task GetCandles()
    {
        throw new NotImplementedException();
    }

}
