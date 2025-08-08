using Microsoft.AspNetCore.Mvc;
using Vertr.Platform.Common.Channels;
using Vertr.Strategies.Contracts;
using Vertr.Strategies.Contracts.Interfaces;

namespace Vertr.Platform.Host.Controllers;
[Route("api/strategies")]
[ApiController]
public class StrategiesController : ControllerBase
{
    private readonly IStrategyMetadataRepository _repository;
    private readonly IDataProducer<StrategyMetadata> _dataProducer;

    public StrategiesController(
        IStrategyMetadataRepository repository,
        IDataProducer<StrategyMetadata> dataProducer)
    {
        _repository = repository;
        _dataProducer = dataProducer;
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll()
    {
        var instruments = await _repository.GetAll();
        return Ok(instruments);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var instrument = await _repository.GetById(id);

        if (instrument == null)
        {
            return NotFound();
        }

        return Ok(instrument);
    }

    [HttpPost()]
    public async Task<IActionResult> Save([FromBody] StrategyMetadata metadata)
    {
        var saved = await _repository.Save(metadata);

        if (!saved)
        {
            return BadRequest($"Cannot save strategy. Id={metadata.Id}");
        }

        await _dataProducer.Produce(metadata);

        return Ok(metadata.Id);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deletedCount = await _repository.Delete(id);
        return Ok();
    }
}
