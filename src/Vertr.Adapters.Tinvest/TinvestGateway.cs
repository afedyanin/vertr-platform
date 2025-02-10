using Tinkoff.InvestApi;
using Vertr.Domain.Ports;
using Vertr.Adapters.Tinvest.Converters;
using Vertr.Domain;

namespace Vertr.Adapters.Tinvest;

internal class TinvestGateway : ITinvestGateway
{
    private readonly InvestApiClient _investApiClient;

    public TinvestGateway(InvestApiClient investApiClient)
    {
        _investApiClient = investApiClient;
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

    public Task GetCandles()
    {
        throw new NotImplementedException();
    }

    public Task GetInstrument()
    {
        throw new NotImplementedException();
    }
}
