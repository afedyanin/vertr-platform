using Microsoft.AspNetCore.Mvc;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.PortfolioManager.Controllers;
[Route("metadata")]
[ApiController]
public class MetadataController : ControllerBase
{
    [HttpGet("id/{portfolioId}")]
    public async Task<IActionResult> GetByPortfolioId(Guid portfolioId)
    {
        return Ok();
    }

    [HttpGet("account/{accountId}")]
    public async Task<IActionResult> GetByAccountId(string accountId)
    {
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePortfolioRequest request)
    {
        return Ok();
    }

}
