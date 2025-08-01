using Microsoft.AspNetCore.Mvc;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.Platform.Host.Controllers;

[Route("tinvest")]
[ApiController]
public class TinvestGatewayController : ControllerBase
{
    private readonly IMarketDataGateway _marketDataGayeway;

    public TinvestGatewayController(IMarketDataGateway marketDataGayeway)
    {
        _marketDataGayeway = marketDataGayeway;
    }

    [HttpGet("instrument-by-ticker/{classCode}/{ticker}")]
    public async Task<IActionResult> GetInstrumentByTicker(string classCode, string ticker)
    {
        var symbol = new Symbol(classCode, ticker);
        var instrument = await _marketDataGayeway.GetInstrumentBySymbol(symbol);
        return Ok(instrument);
    }

    [HttpGet("instrument-by-id/{instrumentId}")]
    public async Task<IActionResult> GetInstrumentById(Guid instrumentId)
    {
        var instrument = await _marketDataGayeway.GetInstrumentById(instrumentId);
        return Ok(instrument);
    }

    [HttpGet("instrument-find/{query}")]
    public async Task<IActionResult> FindInstrument(string query)
    {
        var instruments = await _marketDataGayeway.FindInstrument(query);
        return Ok(instruments);
    }
}
