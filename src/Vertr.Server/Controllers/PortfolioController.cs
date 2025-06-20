using Microsoft.AspNetCore.Mvc;

namespace Vertr.PortfolioManager.Controllers;

[Route("snapshots")]

[ApiController]
public class PortfolioController : ControllerBase
{
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
