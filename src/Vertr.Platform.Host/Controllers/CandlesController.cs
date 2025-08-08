using Microsoft.AspNetCore.Mvc;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.Platform.Host.Controllers;
[Route("api/candles")]
[ApiController]
public class CandlesController : ControllerBase
{
    private readonly ICandlesRepository _candlesRepository;

    public CandlesController(
        ICandlesRepository candlesRepository)
    {
        _candlesRepository = candlesRepository;
    }

    [HttpGet("{instrumentId:guid}")]
    public async Task<IActionResult> GetById(Guid instrumentId, int limit = 500)
    {
        var candles = await _candlesRepository.Get(instrumentId, limit);

        return Ok(candles);
    }
}
