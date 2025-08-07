using Microsoft.AspNetCore.Mvc;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.Platform.Host.Controllers;
[Route("api/candles-history")]
[ApiController]
public class CandlesHistoryController : ControllerBase
{
    private readonly ICandlesHistoryRepository _candlesRepository;

    public CandlesHistoryController(
        ICandlesHistoryRepository candlesRepository)
    {
        _candlesRepository = candlesRepository;
    }

    [HttpGet("{instrumentId}")]
    public async Task<IActionResult> GetById(Guid instrumentId)
    {
        var history = await _candlesRepository.Get(instrumentId);
        return Ok(history);
    }
}
