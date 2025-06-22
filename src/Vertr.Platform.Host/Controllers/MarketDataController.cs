using Microsoft.AspNetCore.Mvc;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.Platform.Host.Controllers;
[Route("market-data")]
[ApiController]

public class MarketDataController : ControllerBase
{
    private readonly IMarketDataService _marketDataService;

    public MarketDataController(IMarketDataService marketDataService)
    {
        _marketDataService = marketDataService;
    }

    [HttpGet("subsciptions")]
    public async Task<IActionResult> GetSubscriptions()
    {
        var subscriptions = await _marketDataService.GetSubscriptions();
        return Ok(subscriptions);
    }
}
