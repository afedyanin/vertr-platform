using Microsoft.AspNetCore.Mvc;
using Vertr.OrderExecution.Contracts;
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
        var identity = new PortfolioIdentity(accountId, bookId);
        var portfolio = await _portfolioManager.GetPortfolio(identity);
        return Ok(portfolio);
    }
}
