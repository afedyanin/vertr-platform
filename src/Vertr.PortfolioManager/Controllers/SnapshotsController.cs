using MediatR;
using Microsoft.AspNetCore.Mvc;
using Vertr.PortfolioManager.Application.Abstractions;
using Vertr.PortfolioManager.Application.Commands;
using Vertr.PortfolioManager.Converters;

namespace Vertr.PortfolioManager.Controllers;

[Route("snapshots")]

[ApiController]
public class SnapshotsController : ControllerBase
{
    private readonly IPortfolioSnapshotRepository _snapshotRepository;
    private readonly IMediator _mediator;

    public SnapshotsController(
        IPortfolioSnapshotRepository snapshotRepository,
        IMediator mediator)
    {
        _snapshotRepository = snapshotRepository;
        _mediator = mediator;
    }

    [HttpGet("account/{accountId}")]
    public async Task<IActionResult> GetLastByAccountId(string accountId)
    {
        var snapshot = await _snapshotRepository.GetLast(accountId, Guid.Empty);

        if (snapshot == null)
        {
            return NotFound();
        }

        var result = snapshot.Convert();

        return Ok(result);
    }

    [HttpGet("account/{accountId}/id/{portfolioId}")]
    public async Task<IActionResult> GetLast(string accountId, Guid portfolioId)
    {
        var snapshot = await _snapshotRepository.GetLast(accountId, portfolioId);

        if (snapshot == null)
        {
            return NotFound();
        }

        var result = snapshot.Convert();

        return Ok(result);
    }

    [HttpPost("tinvest/{portfolioId}")]
    public async Task<IActionResult> MakeTinvestSnapshot(Guid portfolioId)
    {
        var request = new CreateTinvestPortfolioRequest()
        {
            PortfolioId = portfolioId
        };

        var response = await _mediator.Send(request);
        var snapshot = response.Snapshot;

        if (snapshot == null)
        {
            return NotFound();
        }

        var result = snapshot.Convert();

        return Ok(result);
    }
}
