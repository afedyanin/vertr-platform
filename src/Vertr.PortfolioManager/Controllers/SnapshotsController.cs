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

    [HttpGet("{accountId}")]
    public async Task<IActionResult> GetLast(string accountId)
    {
        var snapshot = await _snapshotRepository.GetLast(accountId);

        if (snapshot == null)
        {
            return NotFound();
        }

        var result = snapshot.Convert();

        return Ok(result);
    }

    [HttpPost("{accountId}")]
    public async Task<IActionResult> MakeSnapshot(string accountId)
    {
        var request = new CreatePortfolioSnapshotRequest()
        {
            AccountId = accountId
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
