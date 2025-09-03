using Microsoft.AspNetCore.Mvc;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.MarketData.WebApi;

[Route("api/instruments")]
[ApiController]

public class InstrumentsController : ControllerBase
{
    private readonly IInstrumentsRepository _instrumentRepository;

    public InstrumentsController(IInstrumentsRepository instrumentRepository)
    {
        _instrumentRepository = instrumentRepository;
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll()
    {
        var instruments = await _instrumentRepository.GetAll();
        return Ok(instruments);
    }

    [HttpGet("{instrumentId:guid}")]
    public async Task<IActionResult> GetById(Guid instrumentId)
    {
        var instrument = await _instrumentRepository.GetById(instrumentId);

        if (instrument == null)
        {
            return NotFound();
        }

        return Ok(instrument);
    }

    [HttpPost()]
    public async Task<IActionResult> Save([FromBody] Instrument instrument)
    {
        var saved = await _instrumentRepository.Save(instrument);
        if (!saved)
        {
            return BadRequest($"Cannot save instrument. Id={instrument.Id}");
        }
        return Ok(instrument.Id);
    }

    [HttpPost("import")]
    public async Task<IActionResult> Import([FromBody] Instrument[] instruments)
    {
        foreach (var instrument in instruments)
        {
            _ = await _instrumentRepository.Save(instrument);
        }

        return Ok();
    }

    [HttpDelete("{instrumentId:guid}")]
    public async Task<IActionResult> Delete(Guid instrumentId)
    {
        var deletedCount = await _instrumentRepository.Delete(instrumentId);
        return Ok();
    }
}
