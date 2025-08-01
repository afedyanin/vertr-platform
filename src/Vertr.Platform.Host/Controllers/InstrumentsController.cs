using Microsoft.AspNetCore.Mvc;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.Platform.Host.Controllers;
[Route("api/instruments")]
[ApiController]

public class InstrumentsController : ControllerBase
{
    private readonly IMarketDataInstrumentRepository _instrumentRepository;

    public InstrumentsController(IMarketDataInstrumentRepository instrumentRepository)
    {
        _instrumentRepository = instrumentRepository;
    }

    [HttpGet()]
    public async Task<IActionResult> GetInstruments()
    {
        var instruments = await _instrumentRepository.GetAll();
        return Ok(instruments);
    }

    [HttpGet("{instrumentId}")]
    public async Task<IActionResult> GetInstrumentById(Guid instrumentId)
    {
        var instrument = await _instrumentRepository.GetById(instrumentId);

        if (instrument == null)
        {
            return NotFound();
        }

        return Ok(instrument);
    }

    [HttpPost()]
    public async Task<IActionResult> SaveInstrument([FromBody] Instrument instrument)
    {
        var saved = await _instrumentRepository.Save(instrument);
        if (!saved)
        {
            return BadRequest($"Cannot save instrument. Id={instrument.Id}");
        }
        return Ok(instrument.Id);
    }

    [HttpDelete("{instrumentId}")]
    public async Task<IActionResult> DeleteInstrument(Guid instrumentId)
    {
        var deletedCount = await _instrumentRepository.Delete(instrumentId);
        return Ok();
    }
}
