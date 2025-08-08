using Microsoft.AspNetCore.Mvc;
using Vertr.Strategies.Contracts.Interfaces;

namespace Vertr.Platform.Host.Controllers;
[Route("api/trading-signals")]
[ApiController]
public class TradingSignalsController : ControllerBase
{
    private readonly ITradingSignalRepository _repository;

    public TradingSignalsController(
        ITradingSignalRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("by-strategy/{strategyId:guid}")]
    public async Task<IActionResult> GetByStrategy(Guid strategyId)
    {
        var signals = await _repository.GetByStrategyId(strategyId);
        return Ok(signals);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var signal = await _repository.GetById(id);

        if (signal == null)
        {
            return NotFound();
        }

        return Ok(signal);
    }
}
