using Microsoft.AspNetCore.Mvc;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.Platform.Host.Controllers;
[Route("market-data")]
[ApiController]

public class MarketDataController : ControllerBase
{
    private readonly IStaticMarketDataProvider _staticMarketDataProvider;

    public MarketDataController(IStaticMarketDataProvider staticMarketDataProvider)
    {
        _staticMarketDataProvider = staticMarketDataProvider;
    }

    [HttpGet("subsciptions")]
    public async Task<IActionResult> GetSubscriptions()
    {
        var subscriptions = await _staticMarketDataProvider.GetSubscriptions();
        return Ok(subscriptions);
    }

    [HttpGet("instruments")]
    public async Task<IActionResult> GetInstruments()
    {
        var instruments = await _staticMarketDataProvider.GetInstruments();
        return Ok(instruments);
    }

    [HttpGet("instrument-by-ticker/{classCode}/{ticker}")]
    public async Task<IActionResult> GetInstrumentByTicker(string classCode, string ticker)
    {
        var identity = new InstrumentIdentity(classCode, ticker);
        var instrument = await _staticMarketDataProvider.GetInstrument(identity);
        return Ok(instrument);
    }

    [HttpGet("instrument-by-id/{instrumentId}")]
    public async Task<IActionResult> GetInstrumentById(string instrumentId)
    {
        var identity = new InstrumentIdentity(Guid.Parse(instrumentId));
        var instrument = await _staticMarketDataProvider.GetInstrument(identity);
        return Ok(instrument);
    }

    [HttpGet("instrument-find/{query}")]
    public async Task<IActionResult> FindInstrument(string query)
    {
        var instruments = await _staticMarketDataProvider.FindInstrument(query);
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
