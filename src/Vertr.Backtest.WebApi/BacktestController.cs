using Microsoft.AspNetCore.Mvc;
using Vertr.Backtest.Contracts.Commands;
using Vertr.Backtest.Contracts.Interfaces;
using Vertr.Platform.Common.Jobs;
using Vertr.Platform.Common.Mediator;

namespace Vertr.Backtest.WebApi;

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
        var backtests = await _backtestRepository.GetAll();
        return Ok(backtests);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var backtest = await _backtestRepository.GetById(id);

        if (backtest == null)
        {
            return NotFound();
        }

        return Ok(backtest);
    }

    [HttpGet("by-portfolio/{portfolioId:guid}")]
    public async Task<IActionResult> GetByPortfolioId(Guid portfolioId)
    {
        var backtest = await _backtestRepository.GetByPortfolioId(portfolioId);

        if (backtest == null)
        {
            return NotFound();
        }

        return Ok(backtest);
    }

    [HttpPost()]
    public async Task<IActionResult> Create([FromBody] CreateBacktestRequest request)
    {
        var response = await _mediator.Send(request);

        return Ok(response.BacktestId);
    }

    [HttpPut("start/{id:guid}")]
    public IActionResult Start(Guid id)
    {
        var request = new RunBacktestJobRequest
        {
            BacktestId = id
        };

        var jobId = _jobScheduler.Enqueue(request);
        return Ok(jobId);
    }

    [HttpPut("cancel/{id:guid}")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var cancelled = await _backtestRepository.Cancel(id);
        return Ok(cancelled);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deletedCount = await _backtestRepository.Delete(id);
        return Ok();
    }
}
