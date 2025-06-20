using Microsoft.AspNetCore.Mvc;

namespace Vertr.PortfolioManager.Controllers;

[Route("snapshots")]

[ApiController]
public class PortfolioController : ControllerBase
{
    [HttpGet("{accountId}")]
    public async Task<IActionResult> GetLast(string accountId, Guid? bookId = null)
    {
        return Ok();
    }

    [HttpGet("history/{accountId}")]
    public async Task<IActionResult> GetHistory(string accountId, Guid? bookId = null, int maxRecords = 100)
    {
        return Ok();
    }

    [HttpPost("{accountId}")]
    public async Task<IActionResult> MakeSnapshot(string accountId, Guid? bookId = null)
    {
        return Ok();
    }
}
