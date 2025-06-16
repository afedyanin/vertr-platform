using MediatR;
using Microsoft.AspNetCore.Mvc;
using Vertr.PortfolioManager.Application.Abstractions;
using Vertr.PortfolioManager.Application.Commands;
using Vertr.PortfolioManager.Converters;

namespace Vertr.PortfolioManager.Controllers;

[Route("snapshots")]

[ApiController]
public class PortfolioController : ControllerBase
{
    private readonly IPortfolioSnapshotRepository _snapshotRepository;
    private readonly IMediator _mediator;

    public PortfolioController(
        IPortfolioSnapshotRepository snapshotRepository,
        IMediator mediator)
    {
        _snapshotRepository = snapshotRepository;
        _mediator = mediator;
    }

    [HttpGet("{accountId}")]
    public async Task<IActionResult> GetLast(string accountId, Guid? bookId = null)
    {
        var snapshot = await _snapshotRepository.GetLast(accountId, bookId);

        if (snapshot == null)
        {
            return NotFound();
        }

        var result = snapshot.Convert();

        return Ok(result);
    }

    [HttpGet("history/{accountId}")]
    public async Task<IActionResult> GetHistory(string accountId, Guid? bookId = null, int maxRecords = 100)
    {
        var snapshot = await _snapshotRepository.GetHistory(accountId, bookId, maxRecords);
        var result = snapshot.Convert();
        return Ok(result);
    }

    [HttpPost("{accountId}")]
    public async Task<IActionResult> MakeSnapshot(string accountId, Guid? bookId = null)
    {
        var request = new CreatePortfolioSnapshotRequest()
        {
            AccountId = accountId,
            BookId = bookId
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
