using Microsoft.AspNetCore.Mvc;
using Vertr.Backtest.Contracts.Commands;
using Vertr.Backtest.Contracts.Interfaces;
using Vertr.Platform.Common.Jobs;
using Vertr.Platform.Common.Mediator;

namespace Vertr.Platform.Host.Controllers;
[Route("api/backtests")]
[ApiController]
public class BacktestController : ControllerBase
{
    private readonly IBacktestRepository _backtestRepository;
    private readonly IMediator _mediator;
    private readonly IJobScheduler _jobScheduler;

    public BacktestController(
        IBacktestRepository backtestRepository,
        IJobScheduler jobScheduler,
        IMediator mediator)
    {
        _backtestRepository = backtestRepository;
        _mediator = mediator;
        _jobScheduler = jobScheduler;
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll()
    {
        var instruments = await _backtestRepository.GetAll();
        return Ok(instruments);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var instrument = await _backtestRepository.GetById(id);

        if (instrument == null)
        {
            return NotFound();
        }

        return Ok(instrument);
    }

    [HttpPost()]
    public async Task<IActionResult> Create([FromBody] CreateBacktestRequest request)
    {
        var response = await _mediator.Send(request);

        return Ok(response.BacktestId);
    }

    [HttpPost("{id:guid}")]
    public IActionResult Run(Guid id)
    {
        var request = new RunBacktestJobRequest
        {
            BacktestId = id
        };

        var jobId = _jobScheduler.Enqueue(request);
        return Ok(jobId);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deletedCount = await _backtestRepository.Delete(id);
        return Ok();
    }
}
