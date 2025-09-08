using Microsoft.AspNetCore.Mvc;
using Vertr.Strategies.Contracts;
using Vertr.Strategies.Contracts.Interfaces;

namespace Vertr.Strategies.WebApi;

[Route("api/strategies")]
[ApiController]
public class StrategiesController : ControllerBase
{
    private readonly IStrategyMetadataRepository _metadataRepository;
    private readonly IStrategyRepository _strategyRepository;

    public StrategiesController(
        IStrategyMetadataRepository metadataRepository,
        IStrategyRepository strategyRepository)
    {
        _metadataRepository = metadataRepository;
        _strategyRepository = strategyRepository;
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll()
    {
        var strategy = await _metadataRepository.GetAll();
        return Ok(strategy);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var strategy = await _metadataRepository.GetById(id);

        if (strategy == null)
        {
            return NotFound();
        }

        return Ok(strategy);
    }

    [HttpGet("by-portfolio/{portfolioId:guid}")]
    public async Task<IActionResult> GetByPortfolioId(Guid portfolioId)
    {
        var strategy = await _metadataRepository.GetByPortfolioId(portfolioId);

        if (strategy == null)
        {
            return NotFound();
        }

        return Ok(strategy);
    }

    [HttpPost()]
    public async Task<IActionResult> Save([FromBody] StrategyMetadata metadata)
    {
        var saved = await _metadataRepository.Save(metadata);

        if (!saved)
        {
            return BadRequest($"Cannot save strategy. Id={metadata.Id}");
        }

        await _strategyRepository.Update(metadata);

        return Ok(metadata.Id);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deletedCount = await _metadataRepository.Delete(id);

        if (deletedCount > 0)
        {
            await _strategyRepository.Delete(id);
        }

        return Ok();
    }
}
