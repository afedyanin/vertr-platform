using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.TinvestGateway.Converters;

namespace Vertr.TinvestGateway.Controllers;
[Route("instruments")]
[ApiController]
public class InstrumentsController :  TinvestControllerBase
{
    public InstrumentsController(
        IOptions<TinvestSettings> options,
        InvestApiClient investApiClient) : base(options, investApiClient)
    {
    }

    [HttpGet("find")]
    public async Task<IActionResult> FindInstrument(string query)
    {
        var request = new Tinkoff.InvestApi.V1.FindInstrumentRequest
        {
            Query = query
        };

        var response = await InvestApiClient.Instruments.FindInstrumentAsync(request);

        var instruments = response.Instruments.ToArray().ToInstruments();

        return Ok(instruments);
    }

    [HttpGet()]
    public async Task<IActionResult> GetInstrument(string ticker, string classCode)
    {
        var request = new Tinkoff.InvestApi.V1.InstrumentRequest
        {
            ClassCode = classCode,
            Id = ticker,
            IdType = Tinkoff.InvestApi.V1.InstrumentIdType.Ticker,
        };

        var response = await InvestApiClient.Instruments.GetInstrumentByAsync(request);
        var instrument = response.Instrument.ToInstrument();

        return Ok(instrument);
    }
}
