using Microsoft.AspNetCore.Mvc;
using Vertr.MarketData.Application.Abstractions;
using Vertr.MarketData.Contracts;
using Vertr.TinvestGateway.Contracts.Interfaces;

namespace Vertr.MarketData.Controllers;
[Route("instruments")]
[ApiController]
public class InstrumentsController : ControllerBase
{
    private readonly IMarketInstrumentsRepository _repository;
    private readonly ITinvestGateway _tinvestGateway;

    public InstrumentsController(
        IMarketInstrumentsRepository repository,
        ITinvestGateway tinvestGateway)
    {
        _repository = repository;
        _tinvestGateway = tinvestGateway;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var instruments = await _repository.GetAll();
        return Ok(instruments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var instrument = await _repository.GetById(id);

        if (instrument == null)
        {
            return NotFound();
        }

        // TODO Convert
        return Ok(instrument);
    }

    [HttpGet("by-symbol/{symbol}")]
    public async Task<IActionResult> GetBySymbol(string symbol)
    {
        var instruments = await _repository.GetBySymbol(symbol);

        // TODO Convert
        return Ok(instruments);
    }

    [HttpPost("by-symbol/{symbol}")]
    public async Task<IActionResult> Create(string symbol, string classCode)
    {
        var found = await _tinvestGateway.GetInstrumentByTicker(symbol, classCode);
        if (found == null)
        {
            return BadRequest($"Cannot find instrument by Symbol={symbol} and ClassCode={classCode}");
        }

        var existing = await _repository.GetBySymbol(symbol);

        var instrument = existing.FirstOrDefault(s => s.ClassCode.Equals(classCode, StringComparison.InvariantCultureIgnoreCase));

        if (instrument == null)
        {
            instrument = new MarketInstrument
            {
                Id = Guid.NewGuid(),
                ClassCode = classCode,
                Symbol = symbol,
                Name = ""
            };
        }

        instrument.Name = found.Name ?? "";
        instrument.Currency = found.Currency;
        instrument.ExternalId = found.Uid;
        instrument.LotSize = found.LotSize;

        var saved = await _repository.Save(instrument);

        if (!saved)
        {
            return BadRequest($"Cannot save instrument Symbol={symbol} ClassCode={classCode}");
        }

        return Ok(instrument);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _repository.Delete(id);
        return Ok(deleted);
    }
}
