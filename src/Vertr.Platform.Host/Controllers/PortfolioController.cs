using Microsoft.AspNetCore.Mvc;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.Platform.Host.Controllers;

[Route("portfolio")]
[ApiController]
public class PortfolioController : ControllerBase
{
    private readonly IPortfolioManager _portfolioManager;

    public PortfolioController(IPortfolioManager portfolioManager)
    {
        _portfolioManager = portfolioManager;
    }

    [HttpGet("{accountId}")]
    public async Task<IActionResult> GetLast(string accountId, Guid? bookId = null)
    {
        var portfolio = await _portfolioManager.GetLastPortfolio(accountId, bookId);
        return Ok(portfolio);
    }

    [HttpGet("history/{accountId}")]
    public async Task<IActionResult> GetHistory(string accountId, Guid? bookId = null, int maxRecords = 100)
    {
        var history = await _portfolioManager.GetPortfolioHistory(accountId, bookId);
        return Ok(history);
    }

    [HttpPost("{accountId}")]
    public async Task<IActionResult> MakeSnapshot(string accountId, Guid? bookId = null)
    {
        var snapshot = await _portfolioManager.MakeSnapshot(accountId, bookId);
        return Ok(snapshot);
    }
}
