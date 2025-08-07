using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Vertr.MarketData.Contracts;
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
    public async Task<IActionResult> Get(Guid instrumentId)
    {
        var history = await _candlesRepository.Get(instrumentId);
        return Ok(history);
    }

    [HttpGet("details/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var item = await _candlesRepository.GetById(id);

        if (item == null)
        {
            return NotFound();
        }

        var json = Encoding.UTF8.GetString(item.Data);
        var candles = JsonSerializer.Deserialize<Candle[]>(json, Common.Utils.JsonOptions.DefaultOptions);
        return Ok(candles);
    }
}
