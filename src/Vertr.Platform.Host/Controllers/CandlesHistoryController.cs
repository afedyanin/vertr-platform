using Microsoft.AspNetCore.Mvc;
using Vertr.MarketData.Contracts.Extensions;
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

    [HttpGet("{instrumentId:guid}")]
    public async Task<IActionResult> Get(Guid instrumentId)
    {
        var history = await _candlesRepository.Get(instrumentId);
        return Ok(history);
    }

    [HttpGet("details/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var item = await _candlesRepository.GetById(id);

        if (item == null)
        {
            return NotFound();
        }

        var candles = item.GetCandles();
        return Ok(candles);
    }
}
