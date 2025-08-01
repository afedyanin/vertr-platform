using Microsoft.AspNetCore.Mvc;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.Platform.Host.Controllers;
[Route("market-data")]
[ApiController]

public class MarketDataController : ControllerBase
{
    private readonly IMarketDataInstrumentRepository _instrumentRepository;

    public MarketDataController(IMarketDataInstrumentRepository instrumentRepository)
    {
        _instrumentRepository = instrumentRepository;
    }

    [HttpGet("instruments")]
    public async Task<IActionResult> GetInstruments()
    {
        var instruments = await _instrumentRepository.GetAll();
        return Ok(instruments);
    }

    /*
    [HttpGet("candles/{classCode}/{ticker}/{interval}")]
    public async Task<IActionResult> GetCandles(
        string classCode,
        string ticker,
        CandleInterval interval,
        DateTime from,
        DateTime to,
        int? limit = 100)
    {
        var identity = new InstrumentIdentity(classCode, ticker);
        var candles = await _staticMarketDataProvider.GetCandles(identity, interval, from, to, limit);
        return Ok(candles);
    }
    */
}
