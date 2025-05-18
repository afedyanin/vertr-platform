using Microsoft.AspNetCore.Mvc;

namespace Vertr.MarketData.Controllers;
[Route("candles")]
[ApiController]
public class CandlesController : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> Get(string symbol)
    {
        await Task.Yield();
        return Ok();
    }
}
